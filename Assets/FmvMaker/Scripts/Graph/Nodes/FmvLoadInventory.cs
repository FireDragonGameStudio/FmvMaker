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

        private FmvData fmvData;
        private GameObject fmvClickablePrefab;
        private GameObject fmvInventoryElementsPanel;

        protected override void Definition() {
            InputTrigger = ControlInput(nameof(InputTrigger), LoadInventory);
        }

        private ControlOutput LoadInventory(Flow flow) {
            GetSceneVariables();
            LoadInventoryItems();

            return OutputTrigger;
        }

        private void GetSceneVariables() {
            fmvData = (Variables.ActiveScene.Get("FmvData") as GameObject).GetComponent<FmvData>();
            fmvClickablePrefab = Variables.ActiveScene.Get("ClickableObjectPrefab") as GameObject;
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
                    itemFacade.OnItemClicked.RemoveAllListeners();
                    itemFacade.OnItemClicked.AddListener(ClickInventoryItem);

                    Debug.Log($"Item {gameDataEntry.Id} was added to inventory");
                }
            }
        }

        private void ClickInventoryItem(ClickableModel clickableModel) {
            FmvGraphElementData fmvGraphElementData = fmvData.gameData[clickableModel.Name];
            OnFmvInventoryClicked.Trigger(fmvGraphElementData);
        }
    }
}