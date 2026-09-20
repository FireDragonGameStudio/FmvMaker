using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents the Set Dialogue Node in the Visual Novel Director tool.
    /// </summary>
    /// <remarks>
    /// Is converted to a <see cref="SetDialogueRuntimeNode"/> for the runtime.
    /// It is decorated with the <see cref="NodeAttribute"/> to specify its category path in the graph item library, icon and title.
    /// </remarks>
    [Serializable]
    [Node(CATEGORY_STR, VisualNovelDirectorGraph.ToolPath + "Assets/Icons/SetDialogueNode.png", "Set Dialogue")]
    internal class SetDialogueNode : VisualNovelNode
    {
        public const string IN_PORT_ACTOR_NAME_NAME = SetDialogueRuntimeNode.IN_PORT_ACTOR_NAME_NAME;
        public const string IN_PORT_ACTOR_SPRITE_NAME = SetDialogueRuntimeNode.IN_PORT_ACTOR_SPRITE_NAME;
        public const string IN_PORT_LOCATION_NAME = SetDialogueRuntimeNode.IN_PORT_LOCATION_NAME;
        public const string IN_PORT_DIALOGUE_NAME = SetDialogueRuntimeNode.IN_PORT_DIALOGUE_NAME;
        const string CATEGORY_STR = "Narrative";
        
        public enum Location
        {
            Left = 0,
            Right = 1
        }
        
        /// <summary>
        /// Initializes the node with a custom subtitle and tooltip.
        /// </summary>
        public override void OnEnable()
        {
            // The subtitle is displayed below the title of the node in the graph, and provides additional context about the node (eg: category).
            Subtitle = CATEGORY_STR;
            DefaultColor = Color.darkKhaki;
            // The tooltip is displayed when hovering over the node in the graph, and provides a brief description of what the node does.
            Tooltip = "Sets the dialogue text along with the actor's name, location and sprite.";
        }

        /// <summary>
        /// Defines the output for the node.
        /// </summary>
        /// <param name="context">The scope to define the node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);

            context.AddInputPort<string>(IN_PORT_ACTOR_NAME_NAME)
                .WithDisplayName("Actor Name")
                .WithTooltip("The name of the actor speaking the dialogue.")
                .Build();
            context.AddInputPort<Sprite>(IN_PORT_ACTOR_SPRITE_NAME)
                .WithDisplayName("Actor Sprite")
                .WithTooltip("The sprite representing the actor speaking the dialogue.")
                .Build();
            context.AddInputPort<Location>(IN_PORT_LOCATION_NAME)
                .WithDisplayName("Actor Location")
                .WithTooltip("The location of the actor on the screen.")
                .Build();
            context.AddInputPort<string>(IN_PORT_DIALOGUE_NAME)
                .AsTextArea(5, 8)
                .WithTooltip("The dialogue text to display for the actor.")
                .Build();
        }
    }
}
