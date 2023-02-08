using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using FmvMaker.Core.Provider;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FmvMaker.Graph {
    [UnitCategory("FmvMaker")]
    public class FmvVideoNode : Unit {

        [DoNotSerialize]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        public ControlOutput OutputTrigger;

        //[DoNotSerialize]
        //[PortLabelHidden]
        //public ValueInput FmvTargetVideo;

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
        private FmvGraphElementData fmvTargetClickable;
        private List<FmvGraphElementData> nodeElements = new List<FmvGraphElementData>();
        private List<GameObject> findables = new List<GameObject>();

        protected override void Definition() {
            InputTrigger = ControlInput(nameof(InputTrigger), TriggerFmvVideoNode);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            //FmvTargetVideo = ValueInput<FmvGraphElementData>(nameof(FmvTargetVideo));

            Clickables.Clear();
            for (var i = 0; i < ClickablesCount; i++) {
                Clickables.Add(ValueInput<FmvGraphElementData>("ClickableTarget0" + i));
            }

            Succession(InputTrigger, OutputTrigger);
        }

        private ControlOutput TriggerFmvVideoNode(Flow flow) {
            inputValueVideoView = Variables.Scene(SceneManager.GetActiveScene()).Get("FmvVideoView") as GameObject;
            //fmvTargetClickable = flow.GetValue<FmvGraphElementData>(FmvTargetVideo);

            fmvTargetClickable = Variables.Scene(SceneManager.GetActiveScene()).Get("CurrentVideoTarget") as FmvGraphElementData;

            fmvVideoView = inputValueVideoView.GetComponent<FmvVideoView>();
            fmvVideoView.OnVideoStarted += OnVideoStarted;
            fmvVideoView.OnVideoFinished += OnVideoFinished;
            fmvVideoView.PrepareAndPlay(new VideoModel() {
                Name = fmvTargetClickable.VideoName,
                IsLooping = fmvTargetClickable.IsLooping,
            });

            nodeElements.Clear();
            for (var i = 0; i < ClickablesCount; i++) {
                nodeElements.Add(flow.GetValue<FmvGraphElementData>(Clickables[i]));
            }

            Debug.Log("Video started event registered for " + fmvTargetClickable.VideoName);

            return OutputTrigger;
        }

        private void OnVideoStarted(VideoModel videoModel) {
            if (fmvTargetClickable.VideoName.Equals(videoModel.Name)) {
                EventBus.Trigger(FmvMakerGraphEventNames.OnFmvMakerVideoStarted,
                    new FmvGraphElementData(videoModel.Name, videoModel.IsLooping, videoModel.RelativeScreenPosition));
            }
        }

        private void OnVideoFinished(VideoModel videoModel) {
            // generate buttons for clicking
            for (var i = 0; i < nodeElements.Count; i++) {
                GameObject targetObject = GameObject.Instantiate(Variables.Scene(SceneManager.GetActiveScene()).Get("ClickableObjectPrefab") as GameObject);
                targetObject.SetActive(true);
                targetObject.transform.SetParent((Variables.Scene(SceneManager.GetActiveScene()).Get("VideoElementsPanel") as GameObject).transform);
                targetObject.transform.localScale = Vector3.one;
                FmvClickableFacade itemFacade = targetObject.GetComponent<FmvClickableFacade>();

                // add the item model from the inputs
                itemFacade.SetItemData(new ClickableModel() {
                    Name = "Click-" + nodeElements[i].VideoName,
                    Description = "Description",
                    PickUpVideo = nodeElements[i].VideoName,
                    UseageVideo = "",
                    IsNavigation = true,
                    IsInInventory = false,
                    WasUsed = false,
                    RelativeScreenPosition = nodeElements[i].RelativeScreenPosition,
                });

                // adding the clicking events
                itemFacade.OnItemClicked.RemoveAllListeners();
                itemFacade.OnItemClicked.AddListener(ClickNavigationTarget);

                findables.Add(targetObject);
            }

            if (fmvTargetClickable.VideoName.Equals(videoModel.Name)) {
                EventBus.Trigger(FmvMakerGraphEventNames.OnFmvMakerVideoFinished, fmvTargetClickable);
            }
        }

        private void ClickNavigationTarget(ClickableModel clickableModel) {
            fmvVideoView.OnVideoStarted -= OnVideoStarted;
            fmvVideoView.OnVideoFinished -= OnVideoFinished;
            for (int i = 0; i < findables.Count; i++) {
                GameObject.Destroy(findables[i]);
            }
            EventBus.Trigger(FmvMakerGraphEventNames.OnFmvMakerClickableClicked,
                nodeElements.FirstOrDefault((nodeElement) => nodeElement.VideoName.Equals(clickableModel.PickUpVideo)));
        }
    }
}