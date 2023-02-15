using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using FmvMaker.Core.Provider;
using System;
using System.Collections.Generic;
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
        private FmvData fmvData;
        private GameObject fmvClickablePrefab;
        private GameObject fmvVideoElementsPanel;
        private GameObject fmvInventoryElementsPanel;

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
            fmvData = (Variables.Scene(SceneManager.GetActiveScene()).Get("FmvData") as GameObject).GetComponent<FmvData>();
            fmvClickablePrefab = Variables.Scene(SceneManager.GetActiveScene()).Get("ClickableObjectPrefab") as GameObject;
            fmvVideoElementsPanel = Variables.Scene(SceneManager.GetActiveScene()).Get("VideoElementsPanel") as GameObject;
            fmvInventoryElementsPanel = Variables.Scene(SceneManager.GetActiveScene()).Get("InventoryElementsPanel") as GameObject;

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
            if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                OnFmvVideoStarted.Trigger(fmvTargetClickable);
            }
        }

        private void OnVideoPaused(VideoModel videoModel, bool isPaused) {
            if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                OnFmvVideoPaused.Trigger(fmvTargetClickable);
            }
        }

        private void OnVideoFinished(VideoModel videoModel) {
            if (!videoModel.IsLooping) {
                // generate buttons for clicking
                findables.Clear();
                for (var i = 0; i < nodeElements.Count; i++) {
                    if (!fmvData.gameData.ContainsKey(nodeElements[i].Id)) {
                        fmvData.gameData.Add(nodeElements[i].Id, nodeElements[i]);
                    }
                    FmvGraphElementData nodeData = fmvData.gameData[nodeElements[i].Id];
                    if (!nodeData.IsInInventory && !nodeData.WasUsed) {
                        GameObject targetObject = GameObject.Instantiate(fmvClickablePrefab);
                        targetObject.SetActive(true);
                        targetObject.transform.SetParent(fmvVideoElementsPanel.transform);
                        targetObject.transform.localScale = Vector3.one;
                        FmvClickableFacade itemFacade = targetObject.GetComponent<FmvClickableFacade>();

                        // add the item model from the inputs
                        itemFacade.SetItemData(nodeData.GetItemModel());

                        // adding the clicking events
                        itemFacade.OnItemClicked.RemoveAllListeners();
                        itemFacade.OnItemClicked.AddListener(ClickNavigationTarget);

                        findables.Add(targetObject);
                    }
                }

                if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                    OnFmvVideoFinished.Trigger(fmvTargetClickable);
                }
            }
        }

        private void OnVideoSkipped(VideoModel videoModel) {
            if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                OnFmvVideoSkipped.Trigger(fmvTargetClickable);
            }
        }

        private void ClickNavigationTarget(ClickableModel clickableModel) {
            var graphElementData = fmvData.gameData[clickableModel.Name];
            if (graphElementData == null) {
                Debug.LogError("No NavigationTarget nodeElement found: " + clickableModel.PickUpVideo);
                return;
            }

            // is the clicked object an item and not already in inventory and not used?
            if (graphElementData.IsItem && !graphElementData.IsInInventory && !graphElementData.WasUsed) {
                graphElementData.IsInInventory = true;
                int targetObjectIndex = nodeElements.FindIndex((item) => item.Id == graphElementData.Id);

                // get item gameobject
                GameObject targetObject = findables[targetObjectIndex];
                targetObject.transform.SetParent(fmvInventoryElementsPanel.transform);
                targetObject.transform.localScale = Vector3.one;

                // get itemfacade for new onclick listener
                FmvClickableFacade itemFacade = targetObject.GetComponent<FmvClickableFacade>();
                itemFacade.OnItemClicked.RemoveAllListeners();
                itemFacade.OnItemClicked.AddListener(ClickInventoryItem);
                // TODO: Add onclick for item usage

                Debug.Log($"Item {graphElementData.VideoTarget} was added to inventory");
            }

            fmvGraphVideos.OnVideoStarted.RemoveListener(OnVideoStarted);
            fmvGraphVideos.OnVideoPaused.RemoveListener(OnVideoPaused);
            fmvGraphVideos.OnVideoFinished.RemoveListener(OnVideoFinished);
            fmvGraphVideos.OnVideoSkipped.RemoveListener(OnVideoSkipped);

            // destroy everything except items in inventory
            for (int i = 0; i < findables.Count; i++) {
                if (!nodeElements[i].IsInInventory) {
                    GameObject.Destroy(findables[i]);
                }
            }

            // update game data
            fmvData.gameData[clickableModel.Name] = graphElementData;
            OnFmvClickableClicked.Trigger(graphElementData);
        }

        private void ClickInventoryItem(ClickableModel clickableModel) {
            var graphElementData = fmvData.gameData[clickableModel.Name];
            if (graphElementData == null) {
                Debug.LogError("No NavigationTarget nodeElement found: " + clickableModel.PickUpVideo);
                return;
            }

            // is the clicked object an item and already in inventory and not used and useable here
            if (graphElementData.IsItem && graphElementData.IsInInventory && !graphElementData.WasUsed
                && CheckForFmvTargetVideo(clickableModel.UsageVideo, fmvTargetClickable.UsageTarget)) {
                graphElementData.IsInInventory = false;
                graphElementData.WasUsed = true;

                int targetObjectIndex = nodeElements.FindIndex((item) => item.Id == graphElementData.Id);
                nodeElements[targetObjectIndex] = graphElementData;


                // destroy everything except items in inventory
                for (int i = 0; i < findables.Count; i++) {
                    if (!nodeElements[i].IsInInventory) {
                        GameObject.Destroy(findables[i]);
                    }
                }

                // update game data
                fmvData.gameData[clickableModel.Name] = graphElementData;
                OnFmvClickableClicked.Trigger(graphElementData);
            }

            Debug.Log($"Item {graphElementData.VideoTarget} was used at {clickableModel.UsageVideo}");
        }

        private bool CheckForFmvTargetVideo(string fmvVideoName, FmvVideoEnum target) {
            FmvVideoEnum result;
            return (Enum.TryParse(fmvVideoName, out result) && result == target);
        }
    }
}