using Unity.VisualScripting;
using UnityEngine.Video;

[UnitTitle("On FmvMaker Video Started")]
[UnitCategory("Events\\FmvMaker")]
public class OnFmvMakerVideoStarted : EventUnit<VideoPlayer> {

    [DoNotSerialize]
    public ValueOutput result { get; private set; }// The Event output data to return when the Event is triggered.

    protected override bool register => true;

    // Add an EventHook with the name of the Event to the list of Visual Scripting Events.
    public override EventHook GetHook(GraphReference reference) {
        return new EventHook(FmvMakerGraphEventNames.OnFmvMakerVideoStarted);
    }

    protected override void Definition() {
        base.Definition();

        result = ValueOutput<VideoPlayer>(nameof(result));
    }

    protected override void AssignArguments(Flow flow, VideoPlayer data) {
        flow.SetValue(result, data);
    }
}
