using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core {
    [UseWithContext(typeof(VideoContextNode))]
    [Serializable]
    public class VideoBlockNode : BlockNode {
        protected override void OnDefineOptions(INodeOptionDefinition context) {
            context.AddNodeOption("NodeId", null, null, true, 0, null, Guid.NewGuid().ToString());
            context.AddNodeOption<string>("Name");
            context.AddNodeOption<VideoClip>("VideoClip", "Video Clip");
            context.AddNodeOption("RelativePosition", "Relative Position (0-1)", "0,0 is in the left lower corner and 1,1 in the upper right corner.", false, 0, null, new Vector2(0.5f, 0.5f));
            context.AddNodeOption("RelativeSize", "Relative Size (0-1)", "The size is always relative to screen size, so 0 means no size and 1 is for full screen widht or height.", false, 0, null, new Vector2(0f, 0f));
        }
        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddOutputPort("out").Build();
        }
    }
}