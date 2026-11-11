using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace FmvMaker.Core {
    // bump the version whenever the runtime data changes, to force a reimport
    [ScriptedImporter(3, FmvMakerGraph.AssetExtension)]
    public class FmvMakerImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            FmvMakerGraph fmvMakerGraph = GraphDatabase.LoadGraphForImporter<FmvMakerGraph>(ctx.assetPath);
            FmvMakerRuntimeGraph runtimeGraph = ScriptableObject.CreateInstance<FmvMakerRuntimeGraph>();

            // load nodes from graph
            var nodeIdMap = FmvMakerNodeFactory.LoadNodeMap(fmvMakerGraph.GetNodes());

            // set Graph Id
            runtimeGraph.GraphId = fmvMakerGraph.ID;

            // set Entry Node and the Start node's own out port
            var startNode = fmvMakerGraph.GetNodes().OfType<StartFmvNode>().FirstOrDefault();
            if (startNode != null) {
                runtimeGraph.EntryNodeId = FmvMakerNodeFactory.GetOutputNodeId(startNode, "out", nodeIdMap);
                runtimeGraph.EntryOutputPortId = startNode.GetOutputPortByName("out")?.ID ?? default;
            }

            // create Nodes using the Factory
            foreach (var node in fmvMakerGraph.GetNodes()) {
                if (node is StartFmvNode || node is EndFmvNode) continue;

                var runtimeNode = FmvMakerNodeFactory.Create(node, nodeIdMap);
                runtimeGraph.AllFmvMakerNodes.Add(runtimeNode);
            }

            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }
    }
}