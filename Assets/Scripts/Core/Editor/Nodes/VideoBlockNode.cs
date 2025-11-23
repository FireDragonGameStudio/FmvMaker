using FmvMaker.Provider;
using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace FmvMaker.Core {
    [UseWithContext(typeof(VideoContextNode))]
    [Serializable]
    public class VideoBlockNode : BlockNode {
        protected override void OnDefineOptions(IOptionDefinitionContext context) {
            context.AddOption<string>("NodeId")
                .WithDisplayName("Node Id")
                .WithDefaultValue(Guid.NewGuid().ToString())
                .ShowInInspectorOnly();

            context.AddOption<string>("Name")
                .WithDisplayName("Node Name");

            context.AddOption<Vector2>("RelativePosition")
                .WithDisplayName("Relative Position (0-1)")
                .WithTooltip("0,0 is in the left lower corner and 1,1 in the upper right corner.")
                .WithDefaultValue(new Vector2(0.5f, 0.5f));

            context.AddOption<Vector2>("RelativeSize")
                .WithDisplayName("Relative Size (0-1)")
                .WithTooltip("The size is always relative to screen size, so 0 means no size and 1 is for full screen widht or height.")
                .WithDefaultValue(new Vector2(0f, 0f));
        }
        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddOutputPort("out").Build();
        }
    }
}