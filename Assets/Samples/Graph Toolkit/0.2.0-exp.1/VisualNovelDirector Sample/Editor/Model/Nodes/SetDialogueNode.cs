using System;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents the Set Dialogue Node in the Visual Novel Director tool.
    /// </summary>
    /// <remarks>
    /// Is converted to a <see cref="SetDialogueRuntimeNode"/> for the runtime.
    /// </remarks>
    [Serializable]
    internal class SetDialogueNode : VisualNovelNode
    {
        public const string IN_PORT_ACTOR_NAME_NAME = "ActorName";
        public const string IN_PORT_ACTOR_SPRITE_NAME = "ActorSprite";
        public const string IN_PORT_LOCATION_NAME = "ActorLocation";
        public const string IN_PORT_DIALOGUE_NAME = "Dialogue";

        public enum Location
        {
            Left = 0,
            Right = 1
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
                .Build();
            context.AddInputPort<Sprite>(IN_PORT_ACTOR_SPRITE_NAME)
                .WithDisplayName("Actor Sprite")
                .Build();
            context.AddInputPort<Location>(IN_PORT_LOCATION_NAME)
                .WithDisplayName("Actor Location")
                .Build();
            context.AddInputPort<string>(IN_PORT_DIALOGUE_NAME)
                .Build();
   
        }
    }
}
