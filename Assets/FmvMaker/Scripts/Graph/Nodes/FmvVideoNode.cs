using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
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
        private FmvGraphVideos fmvGraphVideos;
        private FmvGraphElementData fmvTargetClickable;
        private List<FmvGraphElementData> nodeElements = new List<FmvGraphElementData>();
        private List<GameObject> findables = new List<GameObject>();

        protected override void Definition() {
            InputTrigger = ControlInput(nameof(InputTrigger), TriggerFmvVideoNode);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Clickables.Clear();
            for (var i = 0; i < ClickablesCount; i++) {
                Clickables.Add(ValueInput<FmvGraphElementData>("ClickableTarget0" + i));
            }

            Succession(InputTrigger, OutputTrigger);
        }

        private ControlOutput TriggerFmvVideoNode(Flow flow) {
            inputValueVideoView = Variables.Scene(SceneManager.GetActiveScene()).Get("FmvVideoView") as GameObject;

            fmvTargetClickable = Variables.Scene(SceneManager.GetActiveScene()).Get("CurrentVideoTarget") as FmvGraphElementData;

            fmvGraphVideos = inputValueVideoView.GetComponent<FmvGraphVideos>();
            fmvGraphVideos.OnVideoStarted.AddListener(OnVideoStarted);
            fmvGraphVideos.OnVideoPaused.AddListener(OnVideoPaused);
            fmvGraphVideos.OnVideoFinished.AddListener(OnVideoFinished);
            fmvGraphVideos.OnVideoSkipped.AddListener(OnVideoSkipped);
            fmvGraphVideos.PlayVideo(fmvTargetClickable.GetVideoModel());

            nodeElements.Clear();
            for (var i = 0; i < ClickablesCount; i++) {
                nodeElements.Add(flow.GetValue<FmvGraphElementData>(Clickables[i]));
            }

            return OutputTrigger;
        }

        private void OnVideoStarted(VideoModel videoModel) {
            if (fmvTargetClickable.VideoTarget.Equals(videoModel.Name)) {
                OnFmvVideoStarted.Trigger(fmvTargetClickable);
            }
        }

        private void OnVideoPaused(VideoModel videoModel, bool isPaused) {
            if (fmvTargetClickable.VideoTarget.Equals(videoModel.Name)) {
                OnFmvVideoPaused.Trigger(fmvTargetClickable);
            }
        }

        private void OnVideoFinished(VideoModel videoModel) {
            if (!videoModel.IsLooping) {
                // generate buttons for clicking
                for (var i = 0; i < nodeElements.Count; i++) {
                    if (!nodeElements[i].IsInInventory && !nodeElements[i].WasUsed) {
                        GameObject targetObject = GameObject.Instantiate(Variables.Scene(SceneManager.GetActiveScene()).Get("ClickableObjectPrefab") as GameObject);
                        targetObject.SetActive(true);
                        targetObject.transform.SetParent((Variables.Scene(SceneManager.GetActiveScene()).Get("VideoElementsPanel") as GameObject).transform);
                        targetObject.transform.localScale = Vector3.one;
                        FmvClickableFacade itemFacade = targetObject.GetComponent<FmvClickableFacade>();

                        // add the item model from the inputs
                        itemFacade.SetItemData(nodeElements[i].GetItemModel());

                        // adding the clicking events
                        itemFacade.OnItemClicked.RemoveAllListeners();
                        itemFacade.OnItemClicked.AddListener(ClickNavigationTarget);

                        findables.Add(targetObject);
                    }
                }

                if (fmvTargetClickable.VideoTarget.Equals(videoModel.Name)) {
                    OnFmvVideoFinished.Trigger(fmvTargetClickable);
                }
            }
        }

        private void OnVideoSkipped(VideoModel videoModel) {
            if (fmvTargetClickable.VideoTarget.Equals(videoModel.Name)) {
                OnFmvVideoSkipped.Trigger(fmvTargetClickable);
            }
        }

        private void ClickNavigationTarget(ClickableModel clickableModel) {
            var graphElementData = nodeElements.FirstOrDefault((nodeElement) =>
                nodeElement.VideoTarget.ToString().Equals(clickableModel.PickUpVideo));
            if (graphElementData == null) {
                Debug.LogError("No nodeElement found: " + clickableModel.PickUpVideo);
                return;
            }

            if (graphElementData.IsItem && !graphElementData.IsInInventory && !graphElementData.WasUsed) {
                graphElementData.IsInInventory = true;
            }

            fmvGraphVideos.OnVideoStarted.RemoveListener(OnVideoStarted);
            fmvGraphVideos.OnVideoPaused.RemoveListener(OnVideoPaused);
            fmvGraphVideos.OnVideoFinished.RemoveListener(OnVideoFinished);
            fmvGraphVideos.OnVideoSkipped.RemoveListener(OnVideoSkipped);
            for (int i = 0; i < findables.Count; i++) {
                if (!nodeElements[i].IsInInventory) {
                    GameObject.Destroy(findables[i]);
                }
            }

            OnFmvClickableClicked.Trigger(graphElementData);
        }
    }
}