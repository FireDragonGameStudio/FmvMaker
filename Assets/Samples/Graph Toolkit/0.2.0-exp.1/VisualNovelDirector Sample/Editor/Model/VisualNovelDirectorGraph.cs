using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents the Visual Novel Director graph.
    /// </summary>
    /// <remarks> This is the entry point for the Visual Novel Director tool, it represents a Visual Novel Director graph.
    /// This class extends the Graph Toolkit <see cref="Graph"/> class, which provides a range of default behaviors,
    /// reducing the amount of code required to quickly implement a basic graph tool.
    /// It also provides customization by allowing you to override specific methods and can be decorated by attributes to
    /// tailor the tool's functionality.
    /// For example, in this class, we override the OnGraphChanged method to add our custom handling for graph errors and warnings.
    /// We also decorated it with the <see cref="GraphAttribute"/>, which specifies the asset extension.
    /// </remarks>```
    [Serializable]
    [Graph(AssetExtension)]
    internal class VisualNovelDirectorGraph : Graph
    {
        const string k_graphName = "Visual Novel Graph";

        /// <summary>
        /// The file extension for Visual Novel Director graphs.
        /// </summary>
        /// <remarks> In Unity, the extension is used to select the right importer, so it must be unique.</remarks>
        internal const string AssetExtension = "vnd";

        /// <summary>
        /// Creates a new Visual Novel Director graph asset file in the project window.
        /// </summary>
        /// <remarks>This is also where we add the shortcut to create a new graph from the editor Asset menu.</remarks>
        [MenuItem("Assets/Create/Graph Toolkit Samples/Visual Novel Director Graph")]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<VisualNovelDirectorGraph>(k_graphName);
        }

        /// <summary>
        /// Called when the graph changes.
        /// </summary>
        /// <param name="infos">The GraphLogger object to which errors and warnings are added.</param>
        /// <remarks>
        /// This method is triggered whenever the graph is modified. It calls `CheckGraphErrors` to validate the graph
        /// and report any issues.
        /// </remarks>
        public override void OnGraphChanged(GraphLogger infos)
        {
            base.OnGraphChanged(infos);

            CheckGraphErrors(infos);
        }

        /// <summary>
        /// Checks the graph for errors and warnings and adds them to the result object.
        /// </summary>
        /// <param name="infos">Object implementing <see cref="GraphLogger"/> interface and containing
        /// collected errors and warnings</param>
        /// <remarks>Errors and warnings are reported by adding them to the GraphLogger object,
        /// which is the default reporting mechanism for a Graph Toolkit tool. </remarks>
        void CheckGraphErrors(GraphLogger infos)
        {
            List<StartNode> startNodes = GetNodes().OfType<StartNode>().ToList();

            switch (startNodes.Count)
            {
                case 0:
                    infos.LogError("Add a StartNode in your Visual Novel graph.", this);
                    break;
                case >= 1:
                    {
                        foreach (var startNode in startNodes.Skip(1))
                        {
                            infos.LogWarning($"VisualNovelDirector only supports one StartNode per graph. Only the first created one will be used.", startNode);
                        }
                        break;
                    }
            }
        }

       
    }
}
