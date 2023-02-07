using FmvMaker.Core.Models;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("FmvMaker")]
public class FmvVideoViewNode : Unit {
    [DoNotSerialize]
    public ControlInput InputTrigger;

    [DoNotSerialize]
    public ControlOutput OutputTrigger;

    [DoNotSerialize]
    public List<ValueOutput> argumentPorts { get; } = new List<ValueOutput>();

    [SerializeAs(nameof(argumentCount))]
    private int _argumentCount;

    [DoNotSerialize]
    [Inspectable, UnitHeaderInspectable("Arguments")]
    public int argumentCount {
        get => _argumentCount;
        set => _argumentCount = Mathf.Clamp(value, 0, 10);
    }

    private GameObject inputValueVideoView;

    protected override void Definition() {

        InputTrigger = ControlInput("inputTrigger", (flow) => {
            //inputValueVideoView = flow.GetValue<GameObject>(VideoView);
            //FmvVideoView fmvVideoView = inputValueVideoView.GetComponent<FmvVideoView>();

            //fmvVideoView.OnVideoStarted += OnVideoStarted;

            var test = new GetSceneVariable("");
            var who = test.value;

            Debug.Log("video started event registered");

            return OutputTrigger;
        });
        OutputTrigger = ControlOutput("outputTrigger");

        //VideoView = ValueInput<GameObject>("FmvVideoView");

        for (var i = 0; i < argumentCount; i++) {
            argumentPorts.Add(ValueOutput<object>("argument_" + i));
        }

        //Requirement(VideoView, InputTrigger);
        Succession(InputTrigger, OutputTrigger);
    }

    private void OnVideoStarted(VideoModel obj) {
        EventBus.Trigger(FmvMakerGraphEventNames.OnFmvMakerVideoStarted, 0);
    }
}
