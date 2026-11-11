using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using Unity.GraphToolkit.Editor.GraphVisualization;
using UnityEditor;
using UnityEngine;

namespace FmvMaker.Core {
    /// <summary>
    /// Shows the resulting chance (e.g. "33%") next to every randomizer outcome while editing a graph,
    /// so weights don't have to be converted by hand. Uses a visualization context, so nothing is
    /// saved to the asset. In play mode the runtime visualizer shows the chances instead.
    /// </summary>
    [InitializeOnLoad]
    internal static class FmvRandomizerChancePreview {

        private const double RetryInterval = 0.5;
        private const int MaxRetries = 20;

        private sealed class Entry {
            public Context Context;
            public string AssetPath;
            public int Retries;
            public double NextRetryTime;
            public bool NeedsRefresh;
        }

        private static readonly Dictionary<Hash128, Entry> entries = new();

        static FmvRandomizerChancePreview() {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.update += RetryPending;
            AssemblyReloadEvents.beforeAssemblyReload += DisposeAll;

            // after a domain reload the previews are gone and OnGraphChanged doesn't run again by itself
            EditorApplication.delayCall += RefreshAllGraphs;
        }

        private static void RefreshAllGraphs() {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            foreach (var guid in AssetDatabase.FindAssets($"t:{nameof(FmvMakerRuntimeGraph)}")) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith("." + FmvMakerGraph.AssetExtension)) continue;

                var graph = GraphDatabase.LoadGraph<FmvMakerGraph>(path);
                if (graph != null) {
                    Refresh(graph);
                }
            }
        }

        /// <summary>
        /// Called from <see cref="FmvMakerGraph.OnGraphChanged"/>. The update runs on the next editor tick,
        /// because while the graph window loads, OnGraphChanged can run before the context nodes have
        /// their blocks. This also batches several changes into one update.
        /// </summary>
        public static void Refresh(FmvMakerGraph graph) {
            if (graph == null || EditorApplication.isPlayingOrWillChangePlaymode) return;

            if (!entries.TryGetValue(graph.ID, out var entry)) {
                entry = new Entry { AssetPath = GraphDatabase.GetGraphAssetPath(graph) };
                entries[graph.ID] = entry;
            }
            entry.Retries = 0;
            entry.NeedsRefresh = true;
            entry.NextRetryTime = 0;
        }

        private static void Apply(FmvMakerGraph graph, Entry entry) {
            try {
                if (entry.Context == null || !entry.Context.IsValid) {
                    entry.Context = Registry.CreateVisualizationContext(graph.ID);
                }

                // start from scratch, so removed or rewired outcomes lose their label
                entry.Context.ClearAllVisualization();

                foreach (var randomizer in graph.GetNodes().OfType<VideoRandomizerNode>()) {
                    var blocks = randomizer.BlockNodes.ToList();
                    // same rule as the runtime: only connected outcomes with a positive weight can be picked
                    var totalWeight = blocks
                        .Where(IsConnected)
                        .Sum(FmvMakerNodeFactory.GetWeight);

                    foreach (var block in blocks) {
                        var port = block.GetOutputPortByName("out");
                        if (port == null) continue;

                        var label = !IsConnected(block) ? "not connected"
                            : totalWeight <= 0f ? "0%"
                            : $"{FmvMakerNodeFactory.GetWeight(block) / totalWeight * 100f:0.#}%";
                        entry.Context.GetPortReference(port.ID).SetPreview(label);
                    }
                }

                entry.NeedsRefresh = false;
            } catch (Exception e) when (e is ArgumentException || e is InvalidOperationException) {
                // graph window not ready yet, try again shortly
                entry.NeedsRefresh = entry.Retries++ < MaxRetries;
                entry.NextRetryTime = EditorApplication.timeSinceStartup + RetryInterval;
            }
        }

        private static bool IsConnected(INode block) {
            return block.GetOutputPortByName("out")?.IsConnected ?? false;
        }

        private static void RetryPending() {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            foreach (var entry in entries.Values) {
                if (!entry.NeedsRefresh || EditorApplication.timeSinceStartup < entry.NextRetryTime) continue;

                var graph = GraphDatabase.LoadGraph<FmvMakerGraph>(entry.AssetPath);
                if (graph == null) {
                    entry.NeedsRefresh = false;
                    continue;
                }
                Apply(graph, entry);
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change) {
            // Unity releases every visualization context when the play mode changes
            foreach (var entry in entries.Values) {
                entry.Context = null;
                entry.NeedsRefresh = false;
            }

            if (change == PlayModeStateChange.EnteredEditMode) {
                RefreshAllGraphs();
            }
        }

        private static void DisposeAll() {
            foreach (var entry in entries.Values) {
                try {
                    entry.Context?.Dispose();
                } catch (Exception) {
                    // already released
                }
            }
            entries.Clear();
        }
    }
}
