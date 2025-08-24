using System;
using Unity.GraphToolkit.Editor;
using UnityEngine.Video;

namespace FmvMaker.Core {
    [Serializable]
    public class VideoContextNode : ContextNode {
        protected override void OnDefineOptions(INodeOptionDefinition context) {
            context.AddNodeOption("NodeId", null, null, true, 0, null, Guid.NewGuid().ToString());
            context.AddNodeOption<string>("Name");
            context.AddNodeOption<VideoClip>("VideoClip", "Optional Video Clip", "Optional background video clip. Ideal for looping videos of backgrounds, dialogues, ...");
            context.AddNodeOption<bool>("IsLooping", "Is Looping?");
        }

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort("in").Build();
        }
    }
}