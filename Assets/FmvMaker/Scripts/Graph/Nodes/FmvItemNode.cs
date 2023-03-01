using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    [UnitCategory("FmvMaker")]
    public class FmvItemNode : Unit, IGraphElementWithData {

        [Serialize, Inspectable, UnitHeaderInspectable(nameof(Name))]
        public string Name { get; private set; }

        [Serialize, Inspectable, UnitHeaderInspectable(nameof(VideoTarget))]
        public FmvVideoEnum VideoTarget { get; set; } = FmvVideoEnum.None;

        [Serialize, Inspectable, UnitHeaderInspectable(nameof(UsageTarget))]
        public FmvVideoEnum UsageTarget { get; set; } = FmvVideoEnum.None;

        [Serialize, Inspectable, UnitHeaderInspectable(nameof(IsInInventory))]
        public bool IsInInventory { get; private set; }

        [Serialize, Inspectable, UnitHeaderInspectable(nameof(WasUsed))]
        public bool WasUsed { get; private set; }

        [Serialize, Inspectable, UnitHeaderInspectable(nameof(RelativeScreenPosition))]
        public Vector2 RelativeScreenPosition { get; private set; }

        [DoNotSerialize]
        public ValueOutput FmvGraphElementData { get; private set; }

        public IGraphElementData CreateData() {
            return new FmvGraphElementData(Name, VideoTarget, UsageTarget, IsInInventory, WasUsed, RelativeScreenPosition);
        }

        protected override void Definition() {
            FmvGraphElementData = ValueOutput<FmvGraphElementData>(nameof(FmvGraphElementData), (flow) => {
                return flow.stack.GetElementData<FmvGraphElementData>(this);
            });
        }
    }
}