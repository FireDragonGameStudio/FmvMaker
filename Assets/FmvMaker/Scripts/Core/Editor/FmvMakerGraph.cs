using System;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core {
    [Graph(AssetExtension, GraphOptions.SupportsSubgraphs)]
    [Serializable]
    public class FmvMakerGraph : Graph {
        public const string AssetExtension = "fmvmkr";

        [MenuItem("Assets/Create/FmvMaker/FmvMaker Graph", false)]
        static void CreateAssetFile() {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<FmvMakerGraph>();
        }

        /// <summary>
        /// Validates the graph after every edit and puts error/warning markers on the affected nodes.
        /// Only reads the graph; modifying it here can cause recursive updates.
        /// </summary>
        public override void OnGraphChanged(GraphLogger graphLogger) {
            ValidateStartNodes(graphLogger);

            foreach (var node in GetNodes()) {
                switch (node) {
                    case VideoNode videoNode:
                        ValidateVideoClip(videoNode, graphLogger);
                        ValidateIsReachable(videoNode, graphLogger);
                        if (!IsOutputConnected(videoNode, "out")) {
                            graphLogger.LogWarning($"{Describe(videoNode)} has no connected output. The FMV ends after this video. Connect it to an End FMV Node if that's intended.", videoNode);
                        }
                        break;

                    case VideoContextNode contextNode:
                        ValidateVideoClip(contextNode, graphLogger);
                        ValidateIsReachable(contextNode, graphLogger);
                        ValidateBlocks(contextNode, graphLogger);
                        break;

                    case VideoContextTimeNode timeNode:
                        ValidateVideoClip(timeNode, graphLogger);
                        ValidateIsReachable(timeNode, graphLogger);
                        ValidateBlocks(timeNode, graphLogger);
                        if (!IsOutputConnected(timeNode, "out")) {
                            graphLogger.LogWarning($"{Describe(timeNode)} has no default output. The FMV ends when the time runs out without a choice.", timeNode);
                        }
                        break;

                    case VideoRandomizerNode randomizerNode:
                        ValidateIsReachable(randomizerNode, graphLogger);
                        ValidateRandomOutcomes(randomizerNode, graphLogger);
                        break;
                }
            }

            FmvRandomizerChancePreview.Refresh(this);
        }

        private static void ValidateRandomOutcomes(VideoRandomizerNode randomizerNode, GraphLogger graphLogger) {
            if (randomizerNode.BlockCount == 0) {
                graphLogger.LogWarning($"{Describe(randomizerNode)} has no outcomes. Add a block for each possible outcome.", randomizerNode);
                return;
            }

            var pickableWeight = 0f;
            foreach (var block in randomizerNode.BlockNodes) {
                var weight = FmvMakerNodeFactory.GetWeight(block);
                if (!IsOutputConnected(block, "out")) {
                    graphLogger.LogWarning($"Outcome '{GetOption<string>(block, "Name")}' in {Describe(randomizerNode)} has no connected output and is never picked.", block);
                } else if (weight <= 0f) {
                    graphLogger.Log($"Outcome '{GetOption<string>(block, "Name")}' in {Describe(randomizerNode)} has weight 0 and is never picked.", block);
                } else {
                    pickableWeight += weight;
                }
            }

            if (pickableWeight <= 0f) {
                graphLogger.LogError($"{Describe(randomizerNode)} has no outcome that can be picked. The FMV ends here.", randomizerNode);
            }
        }

        private void ValidateStartNodes(GraphLogger graphLogger) {
            var startNodes = GetNodes().OfType<StartFmvNode>().ToList();

            if (startNodes.Count == 0) {
                graphLogger.LogError("The graph has no Start FMV Node, so nothing will play.");
                return;
            }

            foreach (var extraStart in startNodes.Skip(1)) {
                graphLogger.LogError("Only one Start FMV Node is allowed. This one is ignored.", extraStart);
            }

            if (!IsOutputConnected(startNodes[0], "out")) {
                graphLogger.LogError("The Start FMV Node isn't connected, so nothing will play.", startNodes[0]);
            }
        }

        private static void ValidateVideoClip(Node node, GraphLogger graphLogger) {
            if (GetOption<VideoClip>(node, "VideoClip") == null) {
                graphLogger.LogError($"{Describe(node)} has no Video Clip assigned.", node);
            }
        }

        private static void ValidateIsReachable(INode node, GraphLogger graphLogger) {
            var input = node.GetInputPortByName("in");
            if (input != null && !input.IsConnected) {
                graphLogger.LogWarning($"{Describe(node)} has no connected input and can never be reached.", node);
            }
        }

        private static void ValidateBlocks(ContextNode contextNode, GraphLogger graphLogger) {
            if (contextNode.BlockCount == 0) {
                graphLogger.LogWarning($"{Describe(contextNode)} has no choices. Add a block for each choice.", contextNode);
                return;
            }

            foreach (var block in contextNode.BlockNodes) {
                if (!IsOutputConnected(block, "out")) {
                    graphLogger.LogWarning($"Choice '{GetOption<string>(block, "Name")}' in {Describe(contextNode)} has no connected output and is never shown.", block);
                }

                var position = GetOption<Vector2>(block, "RelativePosition");
                var size = GetOption<Vector2>(block, "RelativeSize");
                if (!IsNormalized(position) || !IsNormalized(size)) {
                    graphLogger.LogWarning($"Choice '{GetOption<string>(block, "Name")}' in {Describe(contextNode)} has a relative position or size outside 0-1 and may be off screen.", block);
                }
            }
        }

        private static bool IsOutputConnected(INode node, string portName) {
            return node.GetOutputPortByName(portName)?.IsConnected ?? false;
        }

        private static bool IsNormalized(Vector2 value) {
            return value.x >= 0f && value.x <= 1f && value.y >= 0f && value.y <= 1f;
        }

        private static string Describe(INode node) {
            var name = GetOption<string>(node, "Name");
            return string.IsNullOrWhiteSpace(name) ? $"'{node.GetType().Name}'" : $"'{name}'";
        }

        private static T GetOption<T>(INode node, string optionName) {
            var option = (node as Node)?.GetNodeOptionByName(optionName);
            if (option == null) return default;

            option.TryGetValue(out T value);
            return value;
        }
    }
}
