using System;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.TextureMaker.Editor
{
    /// <summary>
    /// Represents the Texture Maker graph.
    /// </summary>
    /// <remarks> This is the entry point for the Texture Maker tool, it represents a Texture Maker graph.
    /// This class extends the Graph Toolkit <see cref="Graph"/> class, which provides a range of default behaviors,
    /// reducing the amount of code required to quickly implement a basic graph tool.
    /// It also provides customization by allowing you to override specific methods and can be decorated by attributes to
    /// tailor the tool's functionality.
    /// For example, in this class, we override the OnGraphChanged method to add our custom handling for graph errors and warnings.
    /// We also decorated it with the <see cref="GraphAttribute"/>, which specifies the asset extension.
    /// </remarks>
    [Graph(AssetExtension)]
    [Serializable]
    internal class TextureMakerGraph : Graph
    {
        /// <summary>
        /// The file extension for Texture Maker graph assets.
        /// </summary>
        /// <remarks> In Unity, the extension is used to select the right importer, so it must be unique.</remarks>
        internal const string AssetExtension = "texmkr";

        /// <summary>
        /// Create a texture maker asset file.
        /// </summary>
        [MenuItem("Assets/Create/Graph Toolkit Samples/Texture Maker Graph", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<TextureMakerGraph>();
        }

        /// <summary>
        /// Checks the graph for errors and warnings and adds them to the result object.
        /// </summary>
        /// <param name="infos">Object implementing <see cref="GraphLogger"/> interface and containing
        /// collected errors and warnings</param>
        /// <remarks>Errors and warnings are reported by adding them to the GraphLogger object,
        /// which is the default reporting mechanism for a Graph Toolkit tool. </remarks>
        private void CheckGraphErrors(GraphLogger infos)
        {
            // Get all CreateTextureNode instances in the graph
            var createTextureNodes = GetNodes().OfType<CreateTextureNode>().ToList();
            switch (createTextureNodes.Count)
            {
                case 0:
                    // This is a tool design choice to log an error if no CreateTextureNode is found in the graph.
                    // This will result in an error message being logged in the console.
                    infos.LogError("Add a CreateTextureNode in your Texture graph.");
                    break;
                case > 1:
                {
                    foreach (var createTextureNode in createTextureNodes.Skip(1))
                    {
                        // This will result in a warning message being logged in the console and a warning marker being displayed on the node
                        infos.LogWarning($"TextureMaker only supports one {nameof(CreateTextureNode)} by graph. " +
                                         "Only the first created one will be used.", createTextureNode);
                    }
                    break;
                }
            }
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
            CheckGraphErrors(infos);
        }

        /// <summary>
        /// Resolves the value from the given port by determining the type of node connected.
        /// For constant, variable, and texture evaluator nodes, it extracts the corresponding value.
        /// If no connected source is available, it attempts to resolve an embedded value from the port.
        /// </summary>
        /// <typeparam name="T">The expected type of the port value.</typeparam>
        /// <param name="port">The port from which to resolve the value.</param>
        /// <returns>
        /// The resolved value of type <typeparamref name="T"/> if successful; otherwise, the default value of <typeparamref name="T"/>.
        /// </returns>
        public static T ResolvePortValue<T>(IPort port)
        {
            // Get the source port providing input to "port" (null if no connection exists)
            var sourcePort = port.firstConnectedPort;

            switch (sourcePort?.GetNode())
            {
                case IConstantNode node:
                    node.TryGetValue(out T constantValue);
                    return constantValue;
                case IVariableNode node:
                    node.variable.TryGetDefaultValue(out T variableValue);
                    return variableValue;
                case ITextureEvaluatorNode textureEvaluatorNode:
                    if (typeof(T).IsAssignableFrom(typeof(Texture2D)))
                    {
                        return (T)(object)textureEvaluatorNode.EvaluateTexturePort(sourcePort);
                    }
                    break;
                case null:
                    // If no connection exists, try to get "port" 's embedded value (returns type default if unavailable)
                    port.TryGetValue(out T embeddedValue);
                    return embeddedValue;
            }
            return default;
        }
    }
}
