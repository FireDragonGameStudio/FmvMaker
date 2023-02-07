using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using FmvMaker.Core.Provider;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[UnitCategory("FmvMaker")]
public class FmvNode : Unit {

    [DoNotSerialize]
    public ControlInput InputTrigger;

    [DoNotSerialize]
    public ControlOutput OutputTrigger;

    [DoNotSerialize]
    [Inspectable, UnitHeaderInspectable("Video")]
    public FmvVideoEnum FmvTargetVideo { get; private set; }

    [DoNotSerialize]
    public List<ValueInput> Clickables { get; } = new List<ValueInput>();

    [SerializeAs(nameof(ClickablesCount))]
    private int clickablesCount = 2;

    [DoNotSerialize]
    [Inspectable, UnitHeaderInspectable("Clickables")]
    public int ClickablesCount {
        get => clickablesCount;
        set => clickablesCount = Mathf.Clamp(value, 0, 10);
    }

    private GameObject inputValueVideoView;
    private FmvVideoView fmvVideoView;
    private List<ClickableModel> clickables = new List<ClickableModel>();

    protected override void Definition() {
        InputTrigger = ControlInput("inputTrigger", (flow) => {
            inputValueVideoView = Variables.Scene(SceneManager.GetActiveScene()).Get("FmvVideoView") as GameObject;

            fmvVideoView = inputValueVideoView.GetComponent<FmvVideoView>();
            fmvVideoView.OnVideoStarted += OnVideoStarted;
            fmvVideoView.OnVideoFinished += OnVideoFinished;
            fmvVideoView.PrepareAndPlay(new VideoModel() {
                Name = FmvTargetVideo.ToString(),
            });

            clickables.Clear();
            for (var i = 0; i < ClickablesCount; i++) {
                clickables.Add(flow.GetValue<ClickableModel>(Clickables[i]));
            }

            Debug.Log("Video started event registered for " + FmvTargetVideo);

            return OutputTrigger;
        });
        OutputTrigger = ControlOutput("outputTrigger");

        for (var i = 0; i < ClickablesCount; i++) {
            Clickables.Add(ValueInput<object>("Clickable0" + i));
        }
    }

    private void OnVideoStarted(VideoModel obj) {
        if (FmvTargetVideo.ToString().Equals(obj.Name)) {
            EventBus.Trigger(FmvMakerGraphEventNames.OnFmvMakerVideoStarted, 0);
        }
    }

    private void OnVideoFinished(VideoModel obj) {
        // generate buttons for clicking
        for (var i = 0; i < clickables.Count; i++) {
            GameObject targetObject = GameObject.Instantiate(Variables.Scene(SceneManager.GetActiveScene()).Get("ClickableObjectPrefab") as GameObject);
            targetObject.SetActive(true);
            targetObject.transform.SetParent(((GameObject)Variables.Scene(SceneManager.GetActiveScene()).Get("VideoElementsPanel")).transform);
            targetObject.transform.localScale = Vector3.one;
            FmvClickableFacade itemFacade = targetObject.GetComponent<FmvClickableFacade>();

            // add the item model from the inputs
            itemFacade.SetItemData(clickables[i]);

            // adding the clicking events
            itemFacade.OnItemClicked.RemoveAllListeners();
            itemFacade.OnItemClicked.AddListener(TriggerNavigationTarget);
        }
        if (FmvTargetVideo.ToString().Equals(obj.Name)) {
            EventBus.Trigger(FmvMakerGraphEventNames.OnFmvMakerVideoFinished, 0);
        }
    }

    private void TriggerNavigationTarget(ClickableModel model) {
        fmvVideoView.PrepareAndPlay(new VideoModel() {
            Name = model.PickUpVideo,
        });
    }
}
