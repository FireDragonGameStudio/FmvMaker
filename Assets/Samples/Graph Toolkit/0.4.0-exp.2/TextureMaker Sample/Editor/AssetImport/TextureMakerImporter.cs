using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.TextureMaker.Editor
{
    /// <summary>
    /// TextureMakerImporter is a <see cref="ScriptedImporter"/> responsible for creating the texture and adding the
    /// texture to the asset, so the graph can be used directly as a texture in the scene.
    /// It contains all the Texture logic of the tool.
    /// </summary>
    [ScriptedImporter(1, TextureMakerGraph.AssetExtension)]
    internal class TextureMakerImporter : ScriptedImporter
    {
        /// <summary>
        /// The name of the texture identifier in the asset.
        /// </summary>
        private const string OutputTextureAssetIdName = "OutputTexture";

        /// <summary>
        /// Gets or sets the output Texture2D.
        /// </summary>
        private Texture2D OutputTexture { get; set; }

        /// <summary>
        /// Called when the asset is imported. This method processes the graph object and creates the texture.
        /// </summary>
        /// <param name="ctx">The asset import context.</param>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // The loaded graph may be null if the `Graph.LoadForImporter` method fails to load the asset from the
            // specified `ctx.assetPath`.
            // This can occur under the following circumstances:
            // - The asset path is incorrect, or the asset does not exist at the specified location.
            // - The asset located at the specified path is not of type `TextureMakerGraph`.
            // - The asset file itself is problematic, such as being corrupted or stored in an unsupported format.
            //
            // Best practice when dealing with serialization is to account for potential deserialization issues
            // by thoroughly validating and safeguarding against impaired or incomplete data.
            var graph = GraphDatabase.LoadGraphForImporter<TextureMakerGraph>(ctx.assetPath);
            if (graph == null)
            {
                Debug.LogError($"Failed to load texture maker graph object: {ctx.assetPath}");
                return;
            }

            CompileGraph(graph);

            // Adding the texture to the graph object and setting it to be the main asset.
            // This allows the generated texture to be used directly in the project and referenced in a material or a scene.
            if (OutputTexture != null)
            {
                ctx.AddObjectToAsset(OutputTextureAssetIdName, OutputTexture);
                ctx.SetMainObject(OutputTexture);
            }
        }

        /// <summary>
        /// Compiles the provided texture maker graph and updates the output texture based on its evaluated nodes.
        /// </summary>
        /// <param name="graph">The texture maker graph to compile.</param>
        private void CompileGraph(TextureMakerGraph graph)
        {
            // Get the first CreateTexture Node
            // (Only using the first one is a tool design simplification we made for this sample)
            var createTextureNode = graph.GetNodes().OfType<CreateTextureNode>().FirstOrDefault();
            if (createTextureNode == null)
            {
                // No need to log an error here, as an error is already logged in the console (see the `OnGraphChanged` method
                // overriden in the TextureMakerGraph class).
                return;
            }

            // Initialize the output texture to a default texture
            // This enables the graph object to use a default texture when a valid texture evaluator node is not yet available.
            OutputTexture = new Texture2D(CreateTextureNode.OutputWidth, CreateTextureNode.OutputHeight);
            OutputTexture.name = OutputTextureAssetIdName;

            // Get the input port of the CreateTextureNode
            var inputPort = createTextureNode.GetInputPortByName(CreateTextureNode.TextureInputName);
            // Resolve the texture from the input port
            var texture = TextureMakerGraph.ResolvePortValue<Texture2D>(inputPort);
            if (texture != null)
            {
                OutputTexture = new Texture2D(texture.width, texture.height);
                OutputTexture.SetPixels(texture.GetPixels());
            } // else can happen when the input port is not connected and no embedded value is set
        }
    }
}
