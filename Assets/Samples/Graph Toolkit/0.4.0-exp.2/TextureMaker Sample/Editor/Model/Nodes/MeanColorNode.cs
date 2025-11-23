using System;
using JetBrains.Annotations;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.TextureMaker.Editor
{
    /// <summary>
    /// Represents a node that combines two input textures and produces an output texture where pixel values are the mean
    /// of the two input values at the same position.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    internal class MeanColorNode : Node, ITextureEvaluatorNode
    {
        /// Using constants for port ids provides type safety and ensures consistent references across the implementation
        internal const string Texture1InputName = "InTexture1";
        internal const string Texture2InputName = "InTexture2";
        internal const string TextureOutputName = "OutTexture";

        /// <summary>
        /// Defines the input and output port for the Mean node.
        /// This node takes two textures as input and outputs a Texture2D.
        /// </summary>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Texture2D>(Texture1InputName).Build();
            context.AddInputPort<Texture2D>(Texture2InputName).Build();
            context.AddOutputPort<Texture2D>(TextureOutputName).Build();
        }



        /// <summary>
        /// Evaluates the specified port and returns the result.
        /// </summary>
        /// <param name="port">The port model to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        public Texture2D EvaluateTexturePort(IPort port)
        {
            // Only the output port can be evaluated for this node.
            if (port != GetOutputPortByName(TextureOutputName))
                return null;

            // Get the textures from the ports or the embedded value
            Texture2D firstTexture = TextureMakerGraph.ResolvePortValue<Texture2D>(GetInputPortByName(Texture1InputName));
            Texture2D secondTexture = TextureMakerGraph.ResolvePortValue<Texture2D>(GetInputPortByName(Texture2InputName));

            if (firstTexture != null && secondTexture != null)
            {
                // Using min dimensions to prevent out-of-bounds access; simplified for learning (production code would handle varying texture sizes)
                int width = Math.Min(firstTexture.width, secondTexture.width);
                int height = Math.Min(firstTexture.height, secondTexture.height);

                Texture2D resultTexture = new Texture2D(width, height);

                for (var j = 0; j < height; j++)
                {
                    for (var i = 0; i < width; i++)
                    {
                        // Set the color of each pixel in the texture
                        Color firstColor = firstTexture.GetPixel(i, j);
                        Color secondColor = secondTexture.GetPixel(i, j);
                        Color color = (firstColor + secondColor) / 2;
                        resultTexture.SetPixel(i, j, color);
                    }
                }
                return resultTexture;
            }

            // If one of the input ports have no resolved value, return a default texture
            return new Texture2D(CreateTextureNode.OutputWidth, CreateTextureNode.OutputHeight);
        }
    }
}
