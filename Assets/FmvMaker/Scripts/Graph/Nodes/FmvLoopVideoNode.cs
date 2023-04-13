using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using FmvMaker.Core.Provider;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    [UnitCategory("FmvMaker")]
    public class FmvLoopVideoNode : Unit {

        [DoNotSerialize, PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }

        [DoNotSerialize, PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [DoNotSerialize, PortLabel("Wrong Video")]
        public ControlOutput IfFalse { get; private set; }

        [DoNotSerialize]
        public List<ValueInput> Clickables { get; } = new List<ValueInput>();

        [DoNotSerialize]
        public ValueInput FmvTargetVideo { get; private set; }

        [SerializeAs(nameof(ClickablesCount))]
        private int clickablesCount = 2;

        [DoNotSerialize, Inspectable, UnitHeaderInspectable("Clickables")]
        public int ClickablesCount {
            get => clickablesCount;
            set => clickablesCount = Mathf.Clamp(value, 0, 10);
        }

        [Serialize, Inspectable, UnitHeaderInspectable("")]
        public FmvVideoEnum TransitionVideo { get; set; } = FmvVideoEnum.None;

        private GameObject inputValueVideoView;

        private int videoLoopCount = 0;

        private FmvGraphVideos fmvGraphVideos;
        private FmvGraphElementData fmvTargetClickable;
        private FmvData fmvData;
        private GameObject fmvClickablePrefab;
        private GameObject fmvVideoElementsPanel;
        private GameObject fmvInventoryElementsPanel;

        private List<FmvVideoNodeData> nodeData = new List<FmvVideoNodeData>();

        protected override void Definition() {
            InputTrigger = ControlInput(nameof(InputTrigger), TriggerFmvVideoNode);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));
            IfFalse = ControlOutput(nameof(IfFalse));

            Clickables.Clear();
            for (var i = 0; i < ClickablesCount; i++) {
                Clickables.Add(ValueInput<FmvGraphElementData>("ClickableTarget0" + i));
            }

            FmvTargetVideo = ValueInput<FmvGraphElementData>(nameof(FmvTargetVideo));

            Requirement(FmvTargetVideo, InputTrigger);
            Succession(InputTrigger, OutputTrigger);
            Succession(InputTrigger, IfFalse);
        }

        private ControlOutput TriggerFmvVideoNode(Flow flow) {
            GetSceneVariables();

            // reset loop count
            videoLoopCount = 0;

            nodeData.Clear();
            for (int i = 0; i < ClickablesCount; i++) {
                nodeData.Add(new FmvVideoNodeData(null, flow.GetValue<FmvGraphElementData>(Clickables[i])));
            }

            fmvTargetClickable = flow.GetValue<FmvGraphElementData>(FmvTargetVideo);

            fmvGraphVideos = inputValueVideoView.GetComponent<FmvGraphVideos>();
            fmvGraphVideos.OnVideoStarted.AddListener(OnVideoStarted);
            fmvGraphVideos.OnVideoPaused.AddListener(OnVideoPaused);
            fmvGraphVideos.OnVideoFinished.AddListener(OnVideoFinished);
            fmvGraphVideos.OnVideoSkipped.AddListener(OnVideoSkipped);
            fmvGraphVideos.PlayVideo(fmvTargetClickable.GetVideoModel());

            // play navigation and item pickup video
            if (fmvTargetClickable.VideoTarget == TransitionVideo) {
                fmvGraphVideos.PlayVideo(fmvTargetClickable.GetVideoModel());
                return OutputTrigger;
            }

            // play item usage video
            if (fmvTargetClickable.IsItem && fmvTargetClickable.IsInInventory && !fmvTargetClickable.WasUsed && fmvTargetClickable.UsageTarget == TransitionVideo) {
                InventoryItemWasUsedCorrectly(fmvTargetClickable.GetItemModel());
                fmvGraphVideos.PlayVideo(fmvTargetClickable.GetItemUsageVideoModel());
                return OutputTrigger;
            }

            return IfFalse;
        }

        private void OnVideoStarted(VideoModel videoModel) {
            if (!videoModel.IsLooping || videoLoopCount == 0) {
                videoLoopCount++;

                // generate buttons for navigation and item pickup
                for (int i = 0; i < nodeData.Count; i++) {
                    if (!fmvData.gameData.ContainsKey(nodeData[i].Details.Id)) {
                        fmvData.gameData.Add(nodeData[i].Details.Id, nodeData[i].Details);
                    }

                    // get current data for clickable
                    nodeData[i].Details = GetCurrentGameData(nodeData[i].Details.Id);

                    // set all navigation clickables and findable clickables
                    if (!nodeData[i].Details.IsItem || (!nodeData[i].Details.IsInInventory && !nodeData[i].Details.WasUsed)) {

                        // generate clickable object
                        GameObject targetObject = GameObject.Instantiate(fmvClickablePrefab);
                        targetObject.SetActive(true);
                        targetObject.transform.SetParent(fmvVideoElementsPanel.transform);
                        targetObject.transform.localScale = Vector3.one;

                        // get clickable facade
                        FmvClickableFacade itemFacade = targetObject.GetComponent<FmvClickableFacade>();
                        itemFacade.SetItemData(nodeData[i].Details.GetItemModel());

                        // adding the clicking events
                        itemFacade.OnItemClicked.RemoveAllListeners();

                        if (nodeData[i].Details.IsItem) {
                            itemFacade.OnItemClicked.AddListener(ClickPickupItem);
                        } else {
                            itemFacade.OnItemClicked.AddListener(ClickNavigationTarget);
                        }

                        // add to findables
                        nodeData[i].Object = targetObject;
                    }
                }

                // update game data
                if (fmvData.gameData.ContainsKey(videoModel.Name) && !fmvData.gameData[videoModel.Name].AlreadyWatched) {
                    fmvData.gameData[videoModel.Name].AlreadyWatched = true;
                }

                if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                    OnFmvVideoStarted.Trigger(fmvTargetClickable);
                }
            }
        }

        private void OnVideoPaused(VideoModel videoModel, bool isPaused) {
            if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                OnFmvVideoPaused.Trigger(fmvTargetClickable);
            }
        }

        private void OnVideoSkipped(VideoModel videoModel) {
            if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                OnFmvVideoSkipped.Trigger(fmvTargetClickable);
            }
        }

        private void OnVideoFinished(VideoModel videoModel) {
            if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                OnFmvVideoFinished.Trigger(fmvTargetClickable);
            }
        }

        private void ClickNavigationTarget(ClickableModel clickableModel) {
            OnFmvNavigationClicked.Trigger(GetCurrentGameData(clickableModel.Name));
        }

        private void ClickPickupItem(ClickableModel clickableModel) {

            // add item to inventory
            int targetObjectIndex = nodeData.FindIndex((item) => item.Details.Id == clickableModel.Name);

            nodeData[targetObjectIndex].Details.IsInInventory = true;
            nodeData[targetObjectIndex].Object.transform.SetParent(fmvInventoryElementsPanel.transform);
            nodeData[targetObjectIndex].Object.transform.localScale = Vector3.one;

            // set new clicklisteners
            FmvClickableFacade itemFacade = nodeData[targetObjectIndex].Object.transform.GetComponent<FmvClickableFacade>();
            itemFacade.ChangeVisibility(1);
            itemFacade.IsButtonTransparent = false;
            itemFacade.OnItemClicked.RemoveAllListeners();
            itemFacade.OnItemClicked.AddListener(ClickInventoryItem);

            Debug.Log($"Item {nodeData[targetObjectIndex].Details.Id} was added to inventory");

            // update game data
            fmvData.gameData[clickableModel.Name] = nodeData[targetObjectIndex].Details;

            OnFmvItemPickupClicked.Trigger(nodeData[targetObjectIndex].Details);
        }

        private void ClickInventoryItem(ClickableModel clickableModel) {
            FmvGraphElementData fmvGraphElementData = fmvData.gameData[clickableModel.Name];
            OnFmvInventoryClicked.Trigger(fmvGraphElementData);
        }

        private void InventoryItemWasUsedCorrectly(ClickableModel clickableModel) {
            FmvGraphElementData fmvGraphElementData = fmvData.gameData[clickableModel.Name];
            fmvGraphElementData.IsInInventory = false;
            fmvGraphElementData.WasUsed = true;
            // toggle inventory visibility
            fmvInventoryElementsPanel.GetComponentInParent<FmvItemInventoryVisibility>().ToggleInventoryVisibility();

            Debug.Log($"Item {fmvGraphElementData.Id} was used at {fmvTargetClickable.Id} using {fmvGraphElementData.UsageTarget}");

            // update game data
            fmvData.gameData[clickableModel.Name] = fmvGraphElementData;

            // remove item from inventory
            FmvClickableFacade[] fmvInventoryItemObjects = fmvInventoryElementsPanel.GetComponentsInChildren<FmvClickableFacade>();
            foreach (FmvClickableFacade itemFacade in fmvInventoryItemObjects) {
                if (itemFacade.name.Equals(clickableModel.Name)) {
                    itemFacade.OnItemClicked.RemoveAllListeners();
                    GameObject.Destroy(itemFacade.gameObject);
                    break;
                }
            }
        }

        private bool CheckForFmvTargetVideo(string fmvVideoName, FmvVideoEnum target) {
            return (Enum.TryParse(fmvVideoName, out FmvVideoEnum result) && result == target);
        }

        private FmvGraphElementData GetCurrentGameData(string modelName) {
            if (fmvData.gameData.TryGetValue(modelName, out FmvGraphElementData graphElementData)) {
                return graphElementData;
            }
            Debug.LogError($"No NavigationTarget nodeElement found: {modelName}");
            return null;
        }

        private void GetSceneVariables() {
            inputValueVideoView = FmvSceneVariables.VideoView;
            fmvData = FmvSceneVariables.FmvData;
            fmvClickablePrefab = FmvSceneVariables.ClickablePrefab;
            fmvVideoElementsPanel = FmvSceneVariables.VideoElementsPanel;
            fmvInventoryElementsPanel = FmvSceneVariables.InventoryElementsPanel;
        }
    }
}