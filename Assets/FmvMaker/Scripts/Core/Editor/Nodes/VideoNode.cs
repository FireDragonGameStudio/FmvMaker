using FmvMaker.Provider;
using System;
using Unity.GraphToolkit.Editor;
using UnityEngine.Video;

namespace FmvMaker.Core {
    [Serializable]
    public class VideoNode : Node {
        protected override void OnDefineOptions(IOptionDefinitionContext context) {

            context.AddOption<string>("NodeId")
                .WithDefaultValue(Guid.NewGuid().ToString());

            context.AddOption<string>("Name")
                .WithDefaultValue("")
                .Delayed();

            context.AddOption<VideoClip>("VideoClip")
                .WithDisplayName("Video Clip")
                .WithTooltip("Transitional video clip. Ideal for transitioning from one location to another. No UI elements are spawned.")
                .WithDefaultValue(null);

            context.AddOption<FmvInventoryItem>("GivingItem")
                .WithDisplayName("Giving item")
                .WithTooltip("Select the item, that can be found here. Leave blank if no item is provided.");

            context.AddOption<FmvInventoryItem>("NeededItem")
                .WithDisplayName("Needed item")
                .WithTooltip("Select the item, that is needed to go here. Leave blank if no item is needed.");
        }

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort("in")
                .Build();
            context.AddOutputPort("out")
                .Build();

            //var nodeNameOption = GetNodeOptionByName("Name");
            //nodeNameOption.TryGetValue<string>(out string nodeName);
            //context.AddInputPort<string>("Name")
            //    .WithDefaultValue("nodeName")
            //    .Build();

            //var nodeVideoClipOption = GetNodeOptionByName("VideoClip");
            //nodeVideoClipOption.TryGetValue<VideoClip>(out VideoClip nodeVideoClip);
            //context.AddInputPort<VideoClip>("VideoClip")
            //    .WithDisplayName("Video Clip")
            //    .WithDefaultValue(null)
            //    .Build();

            //context.AddInputPort<string>("GivingItem")
            //    .WithDisplayName("Giving Item")
            //    .Build();

            //context.AddInputPort<string>("NeededItem")
            //    .WithDisplayName("Needed Item")
            //    .Build();
        }
    }
}