using System;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents the Wait For Input Node in the Visual Novel Director tool.
    /// <br/><br/>
    /// Use this when you want to have an explicit pause / wait for player input after you change an element
    /// (background, etc.) of the visual novel.
    /// </summary>
    /// <remarks>
    /// Is converted to a <see cref="WaitForInputRuntimeNode"/> for the runtime.
    /// <br/><br/>
    /// The <see cref="SetDialogueNode"/> automatically waits for input after it executes because it creates
    /// a <see cref="WaitForInputRuntimeNode"/> when converted to the runtime graph. This is to match the typical
    /// expected behaviour of visual novel dialogue waiting for player input between dialogue lines.
    /// It is decorated with the <see cref="NodeAttribute"/> to specify its category path in the graph item library, icon and title.
    /// </remarks>
    [Serializable]
    [Node(CATEGORY_STR, VisualNovelDirectorGraph.ToolPath + "Assets/Icons/WaitForInputNode.png", "Wait For Input")]
    internal class WaitForInputNode : VisualNovelNode
    {
        const string CATEGORY_STR = "Flow Control";
        
        /// <summary>
        /// Initializes the node with a custom subtitle and tooltip.
        /// </summary>
        public override void OnEnable()
        {
            // The subtitle is displayed below the title of the node in the graph, and provides additional context about the node (eg: category).
            Subtitle = CATEGORY_STR;
            
            // The tooltip is displayed when hovering over the node in the graph, and provides a brief description of what the node does.
            Tooltip = "Waits for player input before continuing.";
        }
        
        /// <summary>
        /// Defines the output for the node.
        /// </summary>
        /// <param name="context">The scope to define the node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
        }
    }
}
