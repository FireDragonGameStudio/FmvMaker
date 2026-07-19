using FmvMaker.Models;
using FmvMaker.Provider;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core {
    public static class FmvMakerNodeFactory {

        public static Dictionary<INode, string> LoadNodeMap(IEnumerable<INode> graphNodes) {
            var nodeIdMap = new Dictionary<INode, string>();

            foreach (var node in graphNodes) {
                if (node is Node nodeWithOptions) {
                    var nodeId = GetNodeOption<string>(nodeWithOptions.GetNodeOptionByName("NodeId"));
                    nodeIdMap[node] = nodeId;
                }
            }

            return nodeIdMap;
        }

        public static FmvMakerNode Create(INode node, Dictionary<INode, string> nodeIdMap) {
            var runtimeNode = new FmvMakerNode {
                NodeId = nodeIdMap[node],
                NodeName = GetNodeOption<string>((node as Node).GetNodeOptionByName("Name"))
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
            }

            return runtimeNode;
        }

        private static void PopulateVideoNode(VideoNode node, FmvMakerNode runtimeNode, Dictionary<INode, string> nodeIdMap) {
            runtimeNode.VideoClip = GetNodeOption<VideoClip>(node.GetNodeOptionByName("VideoClip"));
            runtimeNode.NeededItem = GetNodeOption<FmvInventoryItem>(node.GetNodeOptionByName("NeededItem"));
            runtimeNode.GivingItem = GetNodeOption<FmvInventoryItem>(node.GetNodeOptionByName("GivingItem"));
            runtimeNode.NextNodeId = GetOutputNodeId(node, "out", nodeIdMap);
        }

        private static void PopulateContextNode(VideoContextNode node, FmvMakerNode runtimeNode, Dictionary<INode, string> nodeIdMap) {
            runtimeNode.VideoClip = GetNodeOption<VideoClip>(node.GetNodeOptionByName("VideoClip"));
            runtimeNode.IsLooping = GetNodeOption<bool>(node.GetNodeOptionByName("IsLooping"));
            runtimeNode.NeededItem = GetNodeOption<FmvInventoryItem>(node.GetNodeOptionByName("NeededItem"));
            runtimeNode.HasDecisionData = node.BlockCount > 0;

            foreach (var block in node.BlockNodes) {
                var nextNodePort = block.GetOutputPortByName("out")?.FirstConnectedPort;
                var blockName = GetNodeOption<string>(block.GetNodeOptionByName("Name"));
                var labelText = GetNodeOption<string>(block.GetNodeOptionByName("Label"));
                var relativePosition = GetNodeOption<Vector2>(block.GetNodeOptionByName("RelativePosition"));
                var relativeSize = GetNodeOption<Vector2>(block.GetNodeOptionByName("RelativeSize"));

                if (nextNodePort != null) {
                    var decisionData = new FmvMakerDecisionData() {
                        DecisionText = blockName,
                        DestinationId = nodeIdMap[nextNodePort.GetNode()],
                        LabelText = labelText,
                        RelativePosition = relativePosition,
                        RelativeSize = relativeSize
                    };

                    runtimeNode.DecisionData.Add(decisionData);
                }
            }
        }

        private static void PopulateContextTimeNode(VideoContextTimeNode node, FmvMakerNode runtimeNode, Dictionary<INode, string> nodeIdMap) {
            runtimeNode.VideoClip = GetNodeOption<VideoClip>(node.GetNodeOptionByName("VideoClip"));
            runtimeNode.NeededItem = GetNodeOption<FmvInventoryItem>(node.GetNodeOptionByName("NeededItem"));
            runtimeNode.IsTimedInteraction = true;
            runtimeNode.HasDecisionData = node.BlockCount > 0;

            foreach (var block in node.BlockNodes) {
                var nextNodePort = block.GetOutputPortByName("out")?.FirstConnectedPort;
                var blockName = GetNodeOption<string>(block.GetNodeOptionByName("Name"));
                var labelText = GetNodeOption<string>(block.GetNodeOptionByName("Label"));
                var relativePosition = GetNodeOption<Vector2>(block.GetNodeOptionByName("RelativePosition"));
                var relativeSize = GetNodeOption<Vector2>(block.GetNodeOptionByName("RelativeSize"));

                if (nextNodePort != null) {
                    var decisionData = new FmvMakerDecisionData() {
                        DecisionText = blockName,
                        DestinationId = nodeIdMap[nextNodePort.GetNode()],
                        LabelText = labelText,
                        RelativePosition = relativePosition,
                        RelativeSize = relativeSize
                    };

                    runtimeNode.DecisionData.Add(decisionData);
                }
            }

            var defaultNodePort = node.GetOutputPortByName("out")?.FirstConnectedPort;
            if (defaultNodePort != null) {
                runtimeNode.NextNodeId = nodeIdMap[defaultNodePort.GetNode()];
            }
        }

        private static string GetOutputNodeId(INode node, string portName, Dictionary<INode, string> nodeIdMap) {
            var port = node.GetOutputPortByName(portName)?.FirstConnectedPort;
            return (port != null && nodeIdMap.ContainsKey(port.GetNode())) ? nodeIdMap[port.GetNode()] : string.Empty;
        }

        private static T GetNodeOption<T>(INodeOption option) {
            if (option == null) return default;

            option.TryGetValue(out T value);
            return value;
        }
    }
}