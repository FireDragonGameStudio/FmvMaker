using System;
using Unity.GraphToolkit.Editor;

namespace FmvMaker.Core {
    /// <summary>
    /// Picks one of its outcome blocks at random (by weight) and continues there.
    /// Plays no video itself, e.g. connect a choice "Rock" to it and add one outcome per opponent video.
    /// </summary>
    [Serializable]
    public class VideoRandomizerNode : ContextNode {
        protected override void OnDefineOptions(IOptionDefinitionContext context) {
            context.AddOption<string>("Name")
                .Delayed();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort("in").Build();
        }
    }
}
