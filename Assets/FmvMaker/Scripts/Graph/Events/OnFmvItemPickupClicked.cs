using Unity.VisualScripting;

namespace FmvMaker.Graph {
    [UnitTitle("On Fmv ItemPickup Clicked")]
    [UnitCategory("Events\\FmvMaker")]
    public class OnFmvItemPickupClicked : EventUnit<FmvGraphElementData> {

        [DoNotSerialize]
        public ValueOutput result { get; private set; } // The Event output data to return when the Event is triggered.

        protected override bool register => true;

        // Add an EventHook with the name of the Event to the list of Visual Scripting Events.
        public override EventHook GetHook(GraphReference reference) {
            return new EventHook(FmvGraphEventNames.OnFmvItemPickupClicked);
        }

        protected override void Definition() {
            base.Definition();
            result = ValueOutput<FmvGraphElementData>(nameof(result));
        }

        protected override void AssignArguments(Flow flow, FmvGraphElementData data) {
            flow.SetValue(result, data);
        }

        public static void Trigger(FmvGraphElementData eventData) {
            EventBus.Trigger(FmvGraphEventNames.OnFmvItemPickupClicked, eventData);
        }
    }
}
