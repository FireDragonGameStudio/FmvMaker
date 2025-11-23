using System;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents the Set Background Node in the Visual Novel Director tool.
    /// </summary>
    /// <remarks>
    /// Is converted to a <see cref="SetBackgroundRuntimeNode"/> for the runtime.
    /// </remarks>
    [Serializable]
    internal class SetBackgroundNode : VisualNovelNode
    {
        public static readonly string IN_PORT_BACKGROUND_NAME = "Background";

        /// <summary>
        /// Defines the output for the node.
        /// </summary>
        /// <param name="context">The scope to define the node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);

            context.AddInputPort<Sprite>(IN_PORT_BACKGROUND_NAME);
        }
    }
}
