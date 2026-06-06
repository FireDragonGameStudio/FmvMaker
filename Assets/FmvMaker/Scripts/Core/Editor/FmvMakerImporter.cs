using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace FmvMaker.Core {
    [ScriptedImporter(1, FmvMakerGraph.AssetExtension)]
    public class FmvMakerImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            FmvMakerGraph fmvMakerGraph = GraphDatabase.LoadGraphForImporter<FmvMakerGraph>(ctx.assetPath);
            FmvMakerRuntimeGraph runtimeGraph = ScriptableObject.CreateInstance<FmvMakerRuntimeGraph>();

            // load nodes from graph
            var nodeIdMap = FmvMakerNodeFactory.LoadNodeMap(fmvMakerGraph.GetNodes());

            // set Entry Node
            runtimeGraph.EntryNodeId = GetEntryNodeId(fmvMakerGraph, nodeIdMap);

            // create Nodes using the Factory
            foreach (var node in fmvMakerGraph.GetNodes()) {
                if (node is StartFmvNode || node is EndFmvNode) continue;

                var runtimeNode = FmvMakerNodeFactory.Create(node, nodeIdMap);
                runtimeGraph.AllFmvMakerNodes.Add(runtimeNode);
            }

            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }

        private string GetEntryNodeId(FmvMakerGraph graph, Dictionary<INode, string> nodeIdMap) {
            var startNode = graph.GetNodes().OfType<StartFmvNode>().FirstOrDefault();
            if (startNode == null) return null;

            // look for the first connected port on the Start node
            var entryPort = startNode.GetOutputPorts().FirstOrDefault()?.FirstConnectedPort;

            if (entryPort != null) {
                var nextNode = entryPort.GetNode();
                if (nodeIdMap.ContainsKey(nextNode)) {
                    return nodeIdMap[nextNode];
                }
            }

            return null;
        }
    }
}