using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents the Set Background Node in the Visual Novel Director tool.
    /// </summary>
    /// <remarks>
    /// Is converted to a <see cref="SetBackgroundRuntimeNode"/> for the runtime.
    /// It is decorated with the <see cref="NodeAttribute"/> to specify its category path in the graph item library, icon and title.
    /// </remarks>
    [Serializable]
    [Node(CATEGORY_STR, VisualNovelDirectorGraph.ToolPath + "Assets/Icons/SetBackgroundNode.png", "Set Background")]
    internal class SetBackgroundNode : VisualNovelNode
    {
        public static readonly string IN_PORT_BACKGROUND_NAME = SetBackgroundRuntimeNode.IN_PORT_BACKGROUND_NAME;
        const string CATEGORY_STR = "Presentation";
        
        /// <summary>
        /// Initializes the node with a custom subtitle and tooltip.
        /// </summary>
        public override void OnEnable()
        {
            // The subtitle is displayed below the title of the node in the graph, and provides additional context about the node (eg: category).
            Subtitle = CATEGORY_STR;
            
            // The tooltip is displayed when hovering over the node in the graph, and provides a brief description of what the node does.
            Tooltip = "Sets the background image of the scene.";
        }
        
        /// <summary>
        /// Defines the output for the node.
        /// </summary>
        /// <param name="context">The scope to define the node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);

            context.AddInputPort<Sprite>(IN_PORT_BACKGROUND_NAME)
                .WithTooltip("The background image to set for the scene.")
                .Build();
        }
    }
}
