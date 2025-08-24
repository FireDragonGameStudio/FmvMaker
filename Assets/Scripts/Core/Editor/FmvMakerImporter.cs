using FmvMaker.Models;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core {
    [ScriptedImporter(1, FmvMakerGraph.AssetExtension)]
    public class FmvMakerImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            FmvMakerGraph fmvMakerGraph = GraphDatabase.LoadGraphForImporter<FmvMakerGraph>(ctx.assetPath);
            FmvMakerRuntimeGraph runtimeGraph = ScriptableObject.CreateInstance<FmvMakerRuntimeGraph>();
            var nodeIdMap = new Dictionary<INode, string>();

            foreach (var node in fmvMakerGraph.GetNodes()) {
                if (node is Node nodeWithOptions) {
                    var nodeId = GetNodeOption<string>(nodeWithOptions.GetNodeOptionByName("NodeId"));
                    nodeIdMap[node] = nodeId;
                }
            }

            // TODO: change that, to load saved node ids as start node
            var startNode = fmvMakerGraph.GetNodes().OfType<StartFmvNode>().FirstOrDefault();
            if (startNode != null) {
                var entryPort = startNode.GetOutputPorts().FirstOrDefault()?.firstConnectedPort;
                if (entryPort != null) {
                    runtimeGraph.EntryNodeId = nodeIdMap[entryPort.GetNode()];
                }
            }

            foreach (var node in fmvMakerGraph.GetNodes()) {
                if (node is StartFmvNode || node is EndFmvNode) continue;

                var runtimeNode = new FmvMakerNode {
                    NodeId = nodeIdMap[node],
                    NodeName = GetNodeOption<string>((node as Node).GetNodeOptionByName("Name"))
                };
                if (node is VideoContextNode videoElementContextNode) {
                    ProcessContextNode(videoElementContextNode, runtimeNode, nodeIdMap);
                }

                runtimeGraph.AllFmvMakerNodes.Add(runtimeNode);
            }

            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }

        private void ProcessContextNode(VideoContextNode node, FmvMakerNode runtimeNode, Dictionary<INode, string> nodeIdMap) {
            runtimeNode.VideoClip = GetNodeOption<VideoClip>(node.GetNodeOptionByName("VideoClip"));
            runtimeNode.IsLooping = GetNodeOption<bool>(node.GetNodeOptionByName("IsLooping"));

            // TODO: use the selected block node for continuation of the graph
            foreach (var block in node.blockNodes) {
                var nextNodePort = block.GetOutputPortByName("out")?.firstConnectedPort;
                var blockName = GetNodeOption<string>(block.GetNodeOptionByName("Name"));
                var videoClip = GetNodeOption<VideoClip>(block.GetNodeOptionByName("VideoClip"));
                var relativePosition = GetNodeOption<Vector2>(block.GetNodeOptionByName("RelativePosition"));
                var relativeSize = GetNodeOption<Vector2>(block.GetNodeOptionByName("RelativeSize"));

                if (nextNodePort != null) {
                    var decisionData = new FmvMakerDecisionData() {
                        DecisionText = blockName,
                        DestinationId = nodeIdMap[nextNodePort.GetNode()],
                        VideoClip = videoClip,
                        RelativePosition = relativePosition,
                    };

                    runtimeNode.DecisionData.Add(decisionData);
                }
            }
            //var nextNodePort = node.GetOutputPortByName("out")?.firstConnectedPort;
            //if (nextNodePort != null) {
            //    runtimeNode.NextNodeId = nodeIdMap[nextNodePort.GetNode()];
            //}
        }

        private T GetPortValue<T>(IPort port) {
            if (port == null) return default(T);

            if (port.isConnected) {
                if (port.firstConnectedPort.GetNode() is IVariableNode variableNode) {
                    variableNode.variable.TryGetDefaultValue(out T value);
                    return value;
                }
            }

            port.TryGetValue(out T fallbackValue);
            return fallbackValue;
        }

        private T GetNodeOption<T>(INodeOption option) {
            if (option == null) return default(T);

            option.TryGetValue(out T value);
            return value;
        }
    }
}