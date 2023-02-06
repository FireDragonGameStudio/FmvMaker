using FmvMaker.Core.Models;
using FmvMaker.Core.Provider;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("FmvMaker")]
public class FmvVideoViewNode : Unit {
    [DoNotSerialize]
    public ControlInput InputTrigger;

    [DoNotSerialize]
    public ControlOutput OutputTrigger;


    [DoNotSerialize]
    public ValueInput VideoView;

    private GameObject inputValueVideoView;

    protected override void Definition() {

        InputTrigger = ControlInput("inputTrigger", (flow) => {
            inputValueVideoView = flow.GetValue<GameObject>(VideoView);
            FmvVideoView fmvVideoView = inputValueVideoView.GetComponent<FmvVideoView>();

            fmvVideoView.OnVideoStarted += OnVideoStarted;

            Debug.Log("video started event registered");

            return OutputTrigger;
        });
        OutputTrigger = ControlOutput("outputTrigger");

        VideoView = ValueInput<GameObject>("FmvVideoView");

        Requirement(VideoView, InputTrigger);
        Succession(InputTrigger, OutputTrigger);
    }

    private void OnVideoStarted(VideoModel obj) {
        EventBus.Trigger(FmvMakerGraphEventNames.OnFmvMakerVideoStarted, 0);
    }
}
