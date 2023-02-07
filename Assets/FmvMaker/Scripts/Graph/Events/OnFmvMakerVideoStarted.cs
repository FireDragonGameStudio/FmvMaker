using FmvMaker.Core.Models;
using Unity.VisualScripting;

[UnitTitle("On FmvMaker Video Started")]
[UnitCategory("Events\\FmvMaker")]
public class OnFmvMakerVideoStarted : EventUnit<VideoModel> {

    [DoNotSerialize]
    public ValueOutput result { get; private set; }// The Event output data to return when the Event is triggered.

    protected override bool register => true;

    // Add an EventHook with the name of the Event to the list of Visual Scripting Events.
    public override EventHook GetHook(GraphReference reference) {
        return new EventHook(FmvMakerGraphEventNames.OnFmvMakerVideoStarted);
    }

    protected override void Definition() {
        base.Definition();

        result = ValueOutput<VideoModel>(nameof(result));
    }

    protected override void AssignArguments(Flow flow, VideoModel data) {
        flow.SetValue(result, data);
    }
}
