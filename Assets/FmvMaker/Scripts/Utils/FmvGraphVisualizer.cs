#if UNITY_EDITOR
using FmvMaker.Core;
using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor.GraphVisualization;
using UnityEngine;

namespace FmvMaker.Utilities {
    /// <summary>
    /// Editor-only play mode feedback for the FmvMaker graph window.
    /// - Wires: every wire of the graph is dimmed; visited wires are drawn thick and fully opaque,
    ///   the wire that was just taken is even thicker and animated.
    /// - Nodes: visited nodes get a full accent bar and a label on their "in" port with the visit count;
    ///   the playing node is animated and labeled "now playing".
    /// Nothing is saved to the graph asset; Unity releases the context when play mode ends.
    /// </summary>
    /// <remarks>
    /// Right after entering play mode the graph window is still rebuilding its view, and the
    /// visualization API throws "Invalid node ID" instead of queuing the request. So this class
    /// keeps the desired state and re-applies it from <see cref="Tick"/> until it succeeds.
    /// </remarks>
    public sealed class FmvGraphVisualizer : IDisposable {

        private const float UnvisitedWireOpacity = 0.25f;
        private const float VisitedWireWidth = 8f;
        private const float ActiveWireWidth = 12f;
        private const float ActiveAnimationSpeed = 1.5f;
        private const float RetryInterval = 0.25f;

        private const string PlayingLabel = "▶ now playing";
        private const string VisitedLabelFormat = "✓ visited {0}×";

        private Context context;
        private Hash128 graphId;
        private bool isEnabled;

        // static graph data
        private readonly HashSet<(Hash128 output, Hash128 input)> allWires = new();
        private readonly Dictionary<Hash128, Hash128> nodeInputPorts = new();
        // randomizer outcome "out" port -> chance (0-1)
        private readonly Dictionary<Hash128, float> outcomeChances = new();
        private readonly Dictionary<Hash128, int> outcomePickCounts = new();

        // run state
        private readonly Dictionary<Hash128, int> visitCounts = new();
        private readonly HashSet<(Hash128 output, Hash128 input)> visitedWires = new();
        private Hash128 activeNodeId;
        private (Hash128 output, Hash128 input)? activeWire;

        // what is currently animating in the graph window, so it can be stopped on the next transition
        private Hash128 playingNodeId;
        private (Hash128 output, Hash128 input)? playingWire;

        private bool isDirty;
        private float nextRetryTime;

        public FmvGraphVisualizer(FmvMakerRuntimeGraph runtimeGraph) {
            if (runtimeGraph == null || !runtimeGraph.GraphId.isValid) return;

            graphId = runtimeGraph.GraphId;
            isEnabled = true;
            CollectGraphData(runtimeGraph);

            // dim the whole graph right away, so the visited path stands out
            Apply();
        }

        // Creating the context can throw as well while the graph window is rebuilding, so it is retried like everything else.
        private bool EnsureContext() {
            if (context != null) return true;

            try {
                context = Registry.CreateVisualizationContext(graphId);
                return true;
            } catch (Exception e) when (e is ArgumentException || e is InvalidOperationException) {
                return false;
            }
        }

        /// <param name="toNodeId">Node that starts playing now.</param>
        /// <param name="fromOutputPortId">Own "out" port of the node or choice block that was taken. May be invalid.</param>
        /// <param name="toInputPortId">Own "in" port of the node that starts playing. May be invalid.</param>
        public void ShowTransition(Hash128 toNodeId, Hash128 fromOutputPortId, Hash128 toInputPortId) {
            if (!isEnabled) return;

            activeNodeId = toNodeId;
            if (toNodeId.isValid) {
                visitCounts.TryGetValue(toNodeId, out var count);
                visitCounts[toNodeId] = count + 1;
            }

            if (outcomeChances.ContainsKey(fromOutputPortId)) {
                outcomePickCounts.TryGetValue(fromOutputPortId, out var picks);
                outcomePickCounts[fromOutputPortId] = picks + 1;
            }

            activeWire = null;
            if (fromOutputPortId.isValid && toInputPortId.isValid) {
                activeWire = (fromOutputPortId, toInputPortId);
                visitedWires.Add(activeWire.Value);
                // wires through portals etc. may not be in the precomputed list
                allWires.Add(activeWire.Value);
            }

            Apply();
        }

        /// <summary>Stops the animations on the current node/wire, keeping the visited trail.</summary>
        public void StopActive() {
            if (!isEnabled) return;

            activeNodeId = default;
            activeWire = null;
            Apply();
        }

        /// <summary>Call every frame; re-applies the visualization if an earlier attempt failed.</summary>
        public void Tick() {
            if (!isEnabled || !isDirty || Time.unscaledTime < nextRetryTime) return;
            Apply();
        }

        public void Dispose() {
            isEnabled = false;
            if (context == null) return;

            try {
                context.Dispose();
            } catch (Exception) {
                // context may already be released by Unity when leaving play mode
            }
            context = null;
        }

        private void CollectGraphData(FmvMakerRuntimeGraph runtimeGraph) {
            var inputPorts = new Dictionary<Hash128, Hash128>();
            foreach (var node in runtimeGraph.AllFmvMakerNodes) {
                inputPorts[node.NodeId] = node.InputPortId;
            }

            void AddWire(Hash128 outputPortId, Hash128 destinationNodeId) {
                if (outputPortId.isValid && inputPorts.TryGetValue(destinationNodeId, out var inputPortId) && inputPortId.isValid) {
                    allWires.Add((outputPortId, inputPortId));
                }
            }

            AddWire(runtimeGraph.EntryOutputPortId, runtimeGraph.EntryNodeId);
            foreach (var node in runtimeGraph.AllFmvMakerNodes) {
                nodeInputPorts[node.NodeId] = node.InputPortId;
                AddWire(node.OutputPortId, node.NextNodeId);
                foreach (var decision in node.DecisionData) {
                    AddWire(decision.OutputPortId, decision.DestinationId);
                    if (node.IsRandomizer && decision.OutputPortId.isValid) {
                        outcomeChances[decision.OutputPortId] = node.GetChance(decision);
                    }
                }
            }
        }

        private void Apply() {
            if (!EnsureContext()) {
                isDirty = true;
                nextRetryTime = Time.unscaledTime + RetryInterval;
                return;
            }

            var ok = true;

            // stop whatever animates but is no longer active
            if (playingNodeId.isValid && playingNodeId != activeNodeId) {
                var id = playingNodeId;
                if (Try(() => context.Motion.Stop(context.GetNodeReference(id)))) playingNodeId = default;
                else ok = false;
            }
            if (playingWire.HasValue && playingWire != activeWire) {
                var wire = playingWire.Value;
                if (Try(() => context.Motion.Stop(context.GetWireReference(wire.output, wire.input)))) playingWire = null;
                else ok = false;
            }

            // wires: dim unvisited, thicken visited, thickest for the one just taken
            foreach (var wire in allWires) {
                var isActive = activeWire.HasValue && activeWire.Value == wire;
                var isVisited = visitedWires.Contains(wire);
                ok &= Try(() => {
                    var wireRef = context.GetWireReference(wire.output, wire.input);
                    wireRef.Opacity = isVisited ? 1f : UnvisitedWireOpacity;
                    // 0 restores the default width
                    wireRef.WidthOverride = isActive ? ActiveWireWidth : isVisited ? VisitedWireWidth : 0f;
                });
            }

            // nodes: full accent bar + label on the "in" port. FillAmount is set before Play,
            // so the bar stays visible after Stop.
            foreach (var visit in visitCounts) {
                var nodeId = visit.Key;
                var label = nodeId == activeNodeId ? PlayingLabel : string.Format(VisitedLabelFormat, visit.Value);
                ok &= Try(() => {
                    var node = context.GetNodeReference(nodeId);
                    node.FillAmount = 100f;
                    if (nodeInputPorts.TryGetValue(nodeId, out var inputPortId) && inputPortId.isValid) {
                        context.GetPortReference(inputPortId).SetPreview(label);
                    }
                });
            }

            // randomizer outcomes: chance and how often each one was picked during this run
            foreach (var outcome in outcomeChances) {
                var portId = outcome.Key;
                outcomePickCounts.TryGetValue(portId, out var picks);
                var label = FormatChance(outcome.Value) + (picks > 0 ? $" · picked {picks}×" : "");
                ok &= Try(() => context.GetPortReference(portId).SetPreview(label));
            }

            // active animations
            if (activeNodeId.isValid && playingNodeId != activeNodeId) {
                var id = activeNodeId;
                if (Try(() => context.Motion.Play(context.GetNodeReference(id), ActiveAnimationSpeed))) playingNodeId = id;
                else ok = false;
            }
            if (activeWire.HasValue && playingWire != activeWire) {
                var wire = activeWire.Value;
                if (Try(() => context.Motion.Play(context.GetWireReference(wire.output, wire.input), ActiveAnimationSpeed))) playingWire = wire;
                else ok = false;
            }

            isDirty = !ok;
            nextRetryTime = Time.unscaledTime + RetryInterval;
        }

        public static string FormatChance(float chance) {
            return $"{chance * 100f:0.#}%";
        }

        // The visualization API throws for IDs it can't resolve yet (graph window not ready, window
        // closed, stale runtime graph). Visualization must never break the game, so swallow those.
        private static bool Try(Action call) {
            try {
                call();
                return true;
            } catch (ArgumentException) {
                return false;
            } catch (InvalidOperationException) {
                return false;
            }
        }
    }
}
#endif
