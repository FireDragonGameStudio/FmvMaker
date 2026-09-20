using System;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents a Two Dialogue Option Node in the Visual Novel Director tool.
    /// </summary>
    /// <remarks>
    /// This node presents two dialogue options to the player and branches the narrative
    /// based on their choice. Is converted to a <see cref="TwoOptionRuntimeNode"/> for the runtime.
    /// It is decorated with the <see cref="NodeAttribute"/> to specify its category path in the graph item library, icon and title.
    /// </remarks>
    [Serializable]
    [Node(CATEGORY_STR, VisualNovelDirectorGraph.ToolPath + "Assets/Icons/TwoOptionNode.png", "Two Options")]
    internal class TwoOptionNode : VisualNovelNode
    {
        public const string IN_PORT_OPTION1_NAME = TwoOptionRuntimeNode.IN_PORT_OPTION1_NAME;
        public const string IN_PORT_OPTION2_NAME = TwoOptionRuntimeNode.IN_PORT_OPTION2_NAME;
        public const string OUT_PORT_OPTION1_NAME = TwoOptionRuntimeNode.OUT_PORT_OPTION1_NAME;
        public const string OUT_PORT_OPTION2_NAME = TwoOptionRuntimeNode.OUT_PORT_OPTION2_NAME;
        const string CATEGORY_STR = "Flow Control";
        const int DialogueTextAreaMinLines = 5;
        const int DialogueTextAreaMaxLines = 8;
        
        /// <summary>
        /// Initializes the node with a custom subtitle and tooltip.
        /// </summary>
        public override void OnEnable()
        {
            // The subtitle is displayed below the title of the node in the graph, and provides additional context about the node (eg: category).
            Subtitle = CATEGORY_STR;
            
            // The tooltip is displayed when hovering over the node in the graph, and provides a brief description of what the node does.
            Tooltip = "Presents two dialogue options to the player and branches the narrative based on their choice.";
        }
        
        /// <summary>
        /// Defines the ports for the node.
        /// </summary>
        /// <param name="context">The scope to define the node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            // Input execution port
            context.AddInputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddInputPort<string>(IN_PORT_OPTION1_NAME)
                .WithDisplayName("Option 1")
                .AsTextArea(DialogueTextAreaMinLines, DialogueTextAreaMaxLines)
                .WithTooltip("The text for the first dialogue option. This will be presented to the player as a choice.")
                .Build();

            context.AddInputPort<string>(IN_PORT_OPTION2_NAME)
                .WithDisplayName("Option 2")
                .AsTextArea(DialogueTextAreaMinLines, DialogueTextAreaMaxLines)
                .WithTooltip("The text for the second dialogue option. This will be presented to the player as a choice.")
                .Build();

            // Two output execution ports for branching
            context.AddOutputPort(OUT_PORT_OPTION1_NAME)
                .WithDisplayName("Option 1")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .AsVertical()
                .Build();

            context.AddOutputPort(OUT_PORT_OPTION2_NAME)
                .WithDisplayName("Option 2")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .AsVertical()
                .Build();
        }
    }
}

