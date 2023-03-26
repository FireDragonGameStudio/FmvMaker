using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using FmvMaker.Core.Provider;
using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    [UnitCategory("FmvMaker")]
    public class FmvLoadInventory : Unit {

        [DoNotSerialize, PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }

        [DoNotSerialize, PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        private FmvGraphVideos fmvGraphVideos;

        private FmvData fmvData;
        private GameObject inputValueVideoView;
        private GameObject fmvClickablePrefab;
        private GameObject fmvVideoElementsPanel;
        private GameObject fmvInventoryElementsPanel;

        protected override void Definition() {
            InputTrigger = ControlInput(nameof(InputTrigger), LoadInventory);
        }

        private ControlOutput LoadInventory(Flow flow) {
            GetSceneVariables();
            LoadInventoryItems();

            fmvGraphVideos = inputValueVideoView.GetComponent<FmvGraphVideos>();

            return OutputTrigger;
        }

        private void GetSceneVariables() {
            fmvData = (Variables.ActiveScene.Get("FmvData") as GameObject).GetComponent<FmvData>();
            inputValueVideoView = Variables.ActiveScene.Get("FmvVideoView") as GameObject;
            fmvClickablePrefab = Variables.ActiveScene.Get("ClickableObjectPrefab") as GameObject;
            fmvVideoElementsPanel = Variables.ActiveScene.Get("VideoElementsPanel") as GameObject;
            fmvInventoryElementsPanel = Variables.ActiveScene.Get("InventoryElementsPanel") as GameObject;
        }

        private void LoadInventoryItems() {
            foreach (FmvGraphElementData gameDataEntry in fmvData.gameData.Values) {
                if (gameDataEntry.IsItem && gameDataEntry.IsInInventory && !gameDataEntry.WasUsed) {
                    // generate clickable object
                    GameObject targetObject = GameObject.Instantiate(fmvClickablePrefab);
                    targetObject.SetActive(true);
                    targetObject.transform.SetParent(fmvInventoryElementsPanel.transform);
                    targetObject.transform.localScale = Vector3.one;

                    // get clickable facade
                    FmvClickableFacade itemFacade = targetObject.GetComponent<FmvClickableFacade>();
                    itemFacade.SetItemData(gameDataEntry.GetItemModel());

                    // adding the clicking events
                    itemFacade.OnItemClicked.RemoveAllListeners();
                    itemFacade.OnItemClicked.AddListener(ClickInventoryItem);
                }
            }
        }

        private void ClickInventoryItem(ClickableModel clickableModel) {
            DestroyGameObjectsExceptInventory();
            RemoveClickListeners();
            FmvGraphElementData fmvGraphElementData = fmvData.gameData[clickableModel.Name];
            OnFmvInventoryClicked.Trigger(fmvGraphElementData);
        }

        private void DestroyGameObjectsExceptInventory() {
            if (fmvVideoElementsPanel) {
                for (int i = 0; i < fmvVideoElementsPanel.transform.childCount; i++) {
                    GameObject.Destroy(fmvVideoElementsPanel.transform.GetChild(i).gameObject);
                }
            }
        }

        private void RemoveClickListeners() {
            fmvGraphVideos.OnVideoStarted.RemoveAllListeners();
            fmvGraphVideos.OnVideoPaused.RemoveAllListeners();
            fmvGraphVideos.OnVideoFinished.RemoveAllListeners();
            fmvGraphVideos.OnVideoSkipped.RemoveAllListeners();
        }
    }
}