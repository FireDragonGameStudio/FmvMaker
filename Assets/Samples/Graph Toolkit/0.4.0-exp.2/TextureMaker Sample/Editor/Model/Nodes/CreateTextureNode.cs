using System;
using JetBrains.Annotations;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.TextureMaker.Editor
{
    /// <summary>
    /// Represents the result node in a Texture Maker Graph.
    /// </summary>
    /// <remarks>
    /// The result node serves as the end node in the texture graph, where the final texture is evaluated.
    /// </remarks>
    [Serializable]
    [UsedImplicitly]
    internal class CreateTextureNode : Node
    {
        /// Using constants for port ids provides type safety and ensures consistent references across the implementation
        internal const string TextureInputName = "InTexture";
        /// <summary>
        /// The width of the output texture.
        /// </summary>
        public const int OutputWidth = 16;

        /// <summary>
        /// The height of the output texture.
        /// </summary>
        public const int OutputHeight = 16;

        /// <summary>
        /// Defines the input for the node.
        /// </summary>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Texture2D>(TextureInputName).Build();
        }
    }
}
