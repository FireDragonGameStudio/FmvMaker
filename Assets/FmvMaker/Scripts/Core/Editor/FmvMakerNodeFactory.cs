using FmvMaker.Models;
using FmvMaker.Provider;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core {
    public static class FmvMakerNodeFactory {

        private const string InPortName = "in";
        private const string OutPortName = "out";

        public static Dictionary<INode, Hash128> LoadNodeMap(IEnumerable<INode> graphNodes) {
            var nodeIdMap = new Dictionary<INode, Hash128>();

            foreach (var node in graphNodes) {
                if (node is Node) {
                    nodeIdMap[node] = node.ID;
                }
            }

            return nodeIdMap;
        }

        public static FmvMakerNode Create(INode node, Dictionary<INode, Hash128> nodeIdMap) {
            var runtimeNode = new FmvMakerNode {
                NodeId = nodeIdMap[node],
                NodeName = GetNodeOption<string>((node as Node).GetNodeOptionByName("Name")),
                // store this node's own port IDs (not the connected ones) for wire visualization
                InputPortId = GetOwnPortId(node.GetInputPortByName(InPortName)),
                OutputPortId = GetOwnPortId(node.GetOutputPortByName(OutPortName))
            };

            switch (node) {
                case VideoNode videoNode:
                    PopulateVideoNode(videoNode, runtimeNode, nodeIdMap);
                    break;
                case VideoContextNode contextNode:
                    PopulateContextNode(contextNode, runtimeNode, nodeIdMap);
                    break;
                case VideoContextTimeNode timeNode:
                    PopulateContextTimeNode(timeNode, runtimeNode, nodeIdMap);
                    break;
                case VideoRandomizerNode randomizerNode:
                    PopulateRandomizerNode(randomizerNode, runtimeNode, nodeIdMap);
                    break;
            }

            return runtimeNode;
        }

        private static void PopulateVideoNode(VideoNode node, FmvMakerNode runtimeNode, Dictionary<INode, Hash128> nodeIdMap) {
            runtimeNode.VideoClip = GetNodeOption<VideoClip>(node.GetNodeOptionByName("VideoClip"));
            runtimeNode.NeededItem = GetNodeOption<FmvInventoryItem>(node.GetNodeOptionByName("NeededItem"));
            runtimeNode.GivingItem = GetNodeOption<FmvInventoryItem>(node.GetNodeOptionByName("GivingItem"));
            runtimeNode.NextNodeId = GetOutputNodeId(node, OutPortName, nodeIdMap);
        }

        private static void PopulateContextNode(VideoContextNode node, FmvMakerNode runtimeNode, Dictionary<INode, Hash128> nodeIdMap) {
            runtimeNode.VideoClip = GetNodeOption<VideoClip>(node.GetNodeOptionByName("VideoClip"));
            runtimeNode.IsLooping = GetNodeOption<bool>(node.GetNodeOptionByName("IsLooping"));
            runtimeNode.NeededItem = GetNodeOption<FmvInventoryItem>(node.GetNodeOptionByName("NeededItem"));
            runtimeNode.HasDecisionData = node.BlockCount > 0;

            AddDecisionData(node, runtimeNode, nodeIdMap);
        }

        private static void PopulateContextTimeNode(VideoContextTimeNode node, FmvMakerNode runtimeNode, Dictionary<INode, Hash128> nodeIdMap) {
            runtimeNode.VideoClip = GetNodeOption<VideoClip>(node.GetNodeOptionByName("VideoClip"));
            runtimeNode.NeededItem = GetNodeOption<FmvInventoryItem>(node.GetNodeOptionByName("NeededItem"));
            runtimeNode.IsTimedInteraction = true;
            runtimeNode.HasDecisionData = node.BlockCount > 0;

            AddDecisionData(node, runtimeNode, nodeIdMap);

            // default option, when the time is up
            runtimeNode.NextNodeId = GetOutputNodeId(node, OutPortName, nodeIdMap);
        }

        private static void PopulateRandomizerNode(VideoRandomizerNode node, FmvMakerNode runtimeNode, Dictionary<INode, Hash128> nodeIdMap) {
            runtimeNode.IsRandomizer = true;
            runtimeNode.HasDecisionData = node.BlockCount > 0;

            foreach (var block in node.BlockNodes) {
                var blockOutPort = block.GetOutputPortByName(OutPortName);
                var nextNodePort = blockOutPort?.FirstConnectedPort;
                if (nextNodePort == null || !nodeIdMap.TryGetValue(nextNodePort.GetNode(), out var destinationId)) {
                    continue;
                }

                runtimeNode.DecisionData.Add(new FmvMakerDecisionData() {
                    DecisionText = GetNodeOption<string>(block.GetNodeOptionByName("Name")),
                    DestinationId = destinationId,
                    OutputPortId = blockOutPort.ID,
                    Weight = GetWeight(block)
                });
            }
        }

        /// <summary>Weight of a randomizer outcome block; never negative.</summary>
        public static float GetWeight(INode block) {
            var option = (block as Node)?.GetNodeOptionByName("Weight");
            if (option == null || !option.TryGetValue(out float weight)) {
                return RandomOutcomeBlockNode.DefaultWeight;
            }
            return Mathf.Max(0f, weight);
        }

        private static void AddDecisionData(ContextNode node, FmvMakerNode runtimeNode, Dictionary<INode, Hash128> nodeIdMap) {
            foreach (var block in node.BlockNodes) {
                var blockOutPort = block.GetOutputPortByName(OutPortName);
                var nextNodePort = blockOutPort?.FirstConnectedPort;
                if (nextNodePort == null || !nodeIdMap.TryGetValue(nextNodePort.GetNode(), out var destinationId)) {
                    continue;
                }

                runtimeNode.DecisionData.Add(new FmvMakerDecisionData() {
                    DecisionText = GetNodeOption<string>(block.GetNodeOptionByName("Name")),
                    DestinationId = destinationId,
                    OutputPortId = blockOutPort.ID,
                    LabelText = GetNodeOption<string>(block.GetNodeOptionByName("Label")),
                    LabelTextColor = GetNodeOption<Color>(block.GetNodeOptionByName("LabelColor")),
                    RelativePosition = GetNodeOption<Vector2>(block.GetNodeOptionByName("RelativePosition")),
                    RelativeSize = GetNodeOption<Vector2>(block.GetNodeOptionByName("RelativeSize"))
                });
            }
        }

        public static Hash128 GetOutputNodeId(INode node, string portName, Dictionary<INode, Hash128> nodeIdMap) {
            var port = node.GetOutputPortByName(portName)?.FirstConnectedPort;
            return (port != null && nodeIdMap.TryGetValue(port.GetNode(), out var id)) ? id : default;
        }

        private static Hash128 GetOwnPortId(IPort port) {
            return port != null ? port.ID : default;
        }

        private static T GetNodeOption<T>(INodeOption option) {
            if (option == null) return default;

            option.TryGetValue(out T value);
            return value;
        }
    }
}
