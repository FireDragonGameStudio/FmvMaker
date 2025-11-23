using System;
using JetBrains.Annotations;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.TextureMaker.Editor
{
    /// <summary>
    /// Represents a node that generates a uniform texture from a specified color.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    internal class UniformNode : Node, ITextureEvaluatorNode
    {
        /// Using constants for port ids provides type safety and ensures consistent references across the implementation
        internal const string ColorInputName = "InColor";
        internal const string TextureOutputName = "OutTexture";
        /// <summary>
        /// Defines the input and output port for the Uniform node.
        /// This nodes takes a color as input and outputs a Texture2D.
        /// </summary>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Color>(ColorInputName).Build();
            context.AddOutputPort<Texture2D>(TextureOutputName).Build();
        }

        /// <summary>
        /// Evaluates the specified port and returns the result.
        /// </summary>
        /// <param name="port">The port model to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        public Texture2D EvaluateTexturePort(IPort port)
        {
            // Only the Texture output port can be evaluated for this node.
            if (port != GetOutputPortByName(TextureOutputName))
                return null;

            // Gets the color port.
            var colorPort = GetInputPortByName(ColorInputName);

            // Get the color from the connected port or the embedded value
            var color = TextureMakerGraph.ResolvePortValue<Color>(colorPort);

            var texture = new Texture2D(CreateTextureNode.OutputWidth, CreateTextureNode.OutputHeight);
            // Set the color of each pixel in the texture
            for (var j = 0; j < CreateTextureNode.OutputHeight; j++)
            {
                for (var i = 0; i < CreateTextureNode.OutputWidth; i++)
                {
                    texture.SetPixel(i, j, color);
                }
            }
            return texture;
        }
    }
}
