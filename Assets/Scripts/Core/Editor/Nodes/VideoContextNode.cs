using System;
using Unity.GraphToolkit.Editor;
using UnityEngine.Video;

namespace FmvMaker.Core {
    [Serializable]
    public class VideoContextNode : ContextNode {
        protected override void OnDefineOptions(IOptionDefinitionContext context) {
            context.AddOption<string>("NodeId")
                .ShowInInspectorOnly()
                .WithDefaultValue(Guid.NewGuid().ToString());

            context.AddOption<string>("Name")
                .Delayed();

            context.AddOption<VideoClip>("VideoClip")
                .WithDisplayName("Video Clip")
                .WithTooltip("Ideal for looping videos of backgrounds, dialogues, ...");

            context.AddOption<bool>("IsLooping")
                .WithDisplayName("Is Looping?");
        }

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort("in").Build();
        }
    }
}