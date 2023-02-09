using Unity.VisualScripting;

namespace FmvMaker.Graph {
    [UnitTitle("On Fmv Clickable Clicked")]
    [UnitCategory("Events\\FmvMaker")]
    public class OnFmvClickableClicked : EventUnit<FmvGraphElementData> {

        [DoNotSerialize]
        public ValueOutput result { get; private set; }// The Event output data to return when the Event is triggered.

        protected override bool register => true;

        // Add an EventHook with the name of the Event to the list of Visual Scripting Events.
        public override EventHook GetHook(GraphReference reference) {
            return new EventHook(FmvGraphEventNames.OnFmvClickableClicked);
        }

        protected override void Definition() {
            base.Definition();

            result = ValueOutput<FmvGraphElementData>(nameof(result));
        }

        protected override void AssignArguments(Flow flow, FmvGraphElementData data) {
            flow.SetValue(result, data);
        }

        public static void Trigger(FmvGraphElementData eventData) {
            EventBus.Trigger(FmvGraphEventNames.OnFmvClickableClicked, eventData);
        }
    }
}