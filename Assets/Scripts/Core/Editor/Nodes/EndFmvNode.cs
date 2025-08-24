using System;
using Unity.GraphToolkit.Editor;

namespace FmvMaker.Core {
    [Serializable]
    public class EndFmvNode : Node {
        protected override void OnDefineOptions(INodeOptionDefinition context) {
            context.AddNodeOption("NodeId", null, null, true, 0, null, Guid.NewGuid().ToString());
            context.AddNodeOption<string>("Name");
        }

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort("in").Build();
        }
    }
}