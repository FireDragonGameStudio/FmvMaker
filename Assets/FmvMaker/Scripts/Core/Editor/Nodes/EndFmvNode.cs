using System;
using Unity.GraphToolkit.Editor;

namespace FmvMaker.Core {
    [Serializable]
    public class EndFmvNode : Node {
        protected override void OnDefineOptions(IOptionDefinitionContext context) {
            context.AddOption<string>("Name");
        }

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort("in").Build();
        }
    }
}