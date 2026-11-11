using System;
using Unity.GraphToolkit.Editor;

namespace FmvMaker.Core {
    /// <summary>
    /// One possible outcome of a <see cref="VideoRandomizerNode"/>.
    /// Weights are relative: equal weights give every outcome the same chance.
    /// </summary>
    [UseWithContext(typeof(VideoRandomizerNode))]
    [Serializable]
    public class RandomOutcomeBlockNode : BlockNode {
        public const float DefaultWeight = 1f;

        protected override void OnDefineOptions(IOptionDefinitionContext context) {
            context.AddOption<string>("Name")
                .WithDisplayName("Outcome Name")
                .Delayed();

            context.AddOption<float>("Weight")
                .WithDisplayName("Weight")
                .WithTooltip("Relative chance of this outcome. Equal weights split the chance evenly, e.g. 1/1/1 = 33% each, 2/1/1 = 50%/25%/25%. 0 disables the outcome.")
                .WithDefaultValue(DefaultWeight)
                .Delayed();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddOutputPort("out").Build();
        }
    }
}
