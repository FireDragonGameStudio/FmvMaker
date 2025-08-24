using System;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents the Start Node in the Visual Novel Director tool.
    /// </summary>
    /// <remarks>
    /// The start node serves as the entry point to the visual novel graph.
    /// </remarks>
    [Serializable]
    internal class StartNode : VisualNovelNode
    {
        /// <summary>
        /// Defines the output for the node.
        /// </summary>
        /// <param name="context">The scope to define the node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            // Start is a special node that has no input, so we don't call DefineCommonPorts
            context.AddOutputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
