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

        private FmvGraphVideos fmvGraphVideos;
        private FmvGraphElementData fmvTargetClickable;
        private FmvData fmvData;
        private GameObject fmvClickablePrefab;
        private GameObject fmvVideoElementsPanel;
        private GameObject fmvInventoryElementsPanel;

        private List<FmvVideoNodeData> nodeData = new List<FmvVideoNodeData>();
        private List<GameObject> inventoryItems = new List<GameObject>();

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

            nodeData.Clear();
            for (int i = 0; i < ClickablesCount; i++) {
                nodeData.Add(new FmvVideoNodeData(null, flow.GetValue<FmvGraphElementData>(Clickables[i])));
            }

            inventoryItems.Clear();

            fmvTargetClickable = flow.GetValue<FmvGraphElementData>(FmvTargetVideo);

            fmvGraphVideos = inputValueVideoView.GetComponent<FmvGraphVideos>();
            fmvGraphVideos.OnVideoStarted.AddListener(OnVideoStarted);
            fmvGraphVideos.OnVideoPaused.AddListener(OnVideoPaused);
            fmvGraphVideos.OnVideoFinished.AddListener(OnVideoFinished);
            fmvGraphVideos.OnVideoSkipped.AddListener(OnVideoSkipped);
            fmvGraphVideos.PlayVideo(fmvTargetClickable.GetVideoModel());

            Variables.Scene(SceneManager.GetActiveScene()).Set("CurrentVideoTarget", fmvTargetClickable);

            // play navigation and item pickup video
            if (fmvTargetClickable.VideoTarget == TransitionVideo) {
                fmvGraphVideos.PlayVideo(fmvTargetClickable.GetVideoModel());
                return OutputTrigger;
            }

            // play item usage video
            if (!fmvTargetClickable.IsInInventory && fmvTargetClickable.WasUsed && fmvTargetClickable.UsageTarget == TransitionVideo) {
                fmvGraphVideos.PlayVideo(fmvTargetClickable.GetItemUsageVideoModel());
                return OutputTrigger;
            }

            return IfFalse;
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

        private void OnVideoSkipped(VideoModel videoModel) {
            if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                OnFmvVideoSkipped.Trigger(fmvTargetClickable);
            }
        }

        private void OnVideoFinished(VideoModel videoModel) {
            if (!videoModel.IsLooping) {
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

                // set click listener for inventory
                FmvClickableFacade[] fmvInventoryItemObjects = fmvInventoryElementsPanel.GetComponentsInChildren<FmvClickableFacade>();
                inventoryItems.Clear();
                foreach (FmvClickableFacade itemFacade in fmvInventoryItemObjects) {
                    itemFacade.OnItemClicked.RemoveAllListeners();
                    itemFacade.OnItemClicked.AddListener(ClickInventoryItem);
                    inventoryItems.Add(itemFacade.gameObject);
                }

                if (CheckForFmvTargetVideo(videoModel.VideoTarget, fmvTargetClickable.VideoTarget)) {
                    OnFmvVideoFinished.Trigger(fmvTargetClickable);
                }
            }
        }

        private void ClickNavigationTarget(ClickableModel clickableModel) {

            RemoveClickListeners();
            DestroyGameObjectsExceptInventory();

            OnFmvNavigationClicked.Trigger(GetCurrentGameData(clickableModel.Name));
        }

        private void ClickPickupItem(ClickableModel clickableModel) {

            RemoveClickListeners();

            // is the clicked object not already in inventory and not used?
            int targetObjectIndex = nodeData.FindIndex((item) => item.Details.Id == clickableModel.Name);
            if (targetObjectIndex > 0 && nodeData[targetObjectIndex].Details.IsItem && !nodeData[targetObjectIndex].Details.IsInInventory && !nodeData[targetObjectIndex].Details.WasUsed) {

                nodeData[targetObjectIndex].Details.IsInInventory = true;
                nodeData[targetObjectIndex].Object.transform.SetParent(fmvInventoryElementsPanel.transform);
                nodeData[targetObjectIndex].Object.transform.localScale = Vector3.one;

                Debug.Log($"Item {nodeData[targetObjectIndex].Details.Id} was added to inventory");
                // update game data
                fmvData.gameData[clickableModel.Name] = nodeData[targetObjectIndex].Details;

                OnFmvItemPickupClicked.Trigger(nodeData[targetObjectIndex].Details);
            }

            DestroyGameObjectsExceptInventory();
        }

        private void ClickInventoryItem(ClickableModel clickableModel) {

            RemoveClickListeners();

            // is the clicked object in inventory and not used?
            int targetObjectIndex = inventoryItems.FindIndex((item) => item.gameObject.name == clickableModel.Name);
            FmvGraphElementData fmvGraphElementData = fmvData.gameData[clickableModel.Name];
            if (targetObjectIndex >= 0 && fmvGraphElementData.IsItem && fmvGraphElementData.IsInInventory && !fmvGraphElementData.WasUsed) {

                fmvGraphElementData.IsInInventory = false;
                fmvGraphElementData.WasUsed = true;

                // destroy after usage
                GameObject.Destroy(inventoryItems[targetObjectIndex]);

                // disable inventory visibility
                fmvInventoryElementsPanel.GetComponentInParent<FmvItemInventoryVisibility>().ToggleInventoryVisibility();

                Debug.Log($"Item {fmvGraphElementData.Id} was used at {fmvTargetClickable.Id} using {fmvGraphElementData.UsageTarget}");
                // update game data
                fmvData.gameData[clickableModel.Name] = fmvGraphElementData;

                OnFmvInventoryClicked.Trigger(fmvGraphElementData);
            }

            DestroyGameObjectsExceptInventory();
        }

        private bool CheckForFmvTargetVideo(string fmvVideoName, FmvVideoEnum target) {
            FmvVideoEnum result;
            return (Enum.TryParse(fmvVideoName, out result) && result == target);
        }

        private FmvGraphElementData GetCurrentGameData(string modelName) {
            var graphElementData = fmvData.gameData[modelName];
            if (graphElementData == null) {
                Debug.LogError($"No NavigationTarget nodeElement found: {modelName}");
            }
            return graphElementData;
        }

        private void GetSceneVariables() {
            inputValueVideoView = Variables.Scene(SceneManager.GetActiveScene()).Get("FmvVideoView") as GameObject;
            fmvData = (Variables.Scene(SceneManager.GetActiveScene()).Get("FmvData") as GameObject).GetComponent<FmvData>();
            fmvClickablePrefab = Variables.Scene(SceneManager.GetActiveScene()).Get("ClickableObjectPrefab") as GameObject;
            fmvVideoElementsPanel = Variables.Scene(SceneManager.GetActiveScene()).Get("VideoElementsPanel") as GameObject;
            fmvInventoryElementsPanel = Variables.Scene(SceneManager.GetActiveScene()).Get("InventoryElementsPanel") as GameObject;
        }

        private void RemoveClickListeners() {
            fmvGraphVideos.OnVideoStarted.RemoveAllListeners();
            fmvGraphVideos.OnVideoPaused.RemoveAllListeners();
            fmvGraphVideos.OnVideoFinished.RemoveAllListeners();
            fmvGraphVideos.OnVideoSkipped.RemoveAllListeners();
        }

        private void DestroyGameObjectsExceptInventory() {
            for (int i = 0; i < nodeData.Count; i++) {
                FmvGraphElementData graphElementDataTemp = GetCurrentGameData(nodeData[i].Details.Id);
                if (!graphElementDataTemp.IsInInventory) {
                    GameObject.Destroy(nodeData[i].Object);
                }
            }
        }
    }
}