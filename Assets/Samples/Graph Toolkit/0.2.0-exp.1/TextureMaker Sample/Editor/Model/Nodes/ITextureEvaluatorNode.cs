using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.TextureMaker.Editor
{
    /// <summary>
    /// Defines an interface for evaluating Texture ports.
    /// </summary>
    /// <remarks>
    /// This interface is defined by nodes with the ability to output a Texture2D.
    /// Implementations of this interface should provide the logic to evaluate a given port model and return the resulting Texture.
    /// </remarks>
    internal interface ITextureEvaluatorNode
    {
        /// <summary>
        /// Evaluates the specified port and returns the resulting Texture.
        /// </summary>
        /// <param name="port">The port to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        public Texture2D EvaluateTexturePort(IPort port);
    }
}
