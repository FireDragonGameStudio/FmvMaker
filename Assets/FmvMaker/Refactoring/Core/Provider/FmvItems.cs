using FmvMaker.Core.Facades;
using FmvMaker.Core.Utilities;
using FmvMaker.Core.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvItems : MonoBehaviour {

        [SerializeField]
        private FmvVideos fmvVideos;
        [SerializeField]
        private RectTransform videoElementsPanel = null;
        [SerializeField]
        private RectTransform inventoryElementsPanel = null;
        [SerializeField]
        private GameObject itemObjectPrefab = null;

        private List<ItemModel> allItems = new List<ItemModel>();
        private List<FmvItemFacade> allInventoryItems = new List<FmvItemFacade>();
        private List<FmvItemFacade> allFindableItems = new List<FmvItemFacade>();

        private void Awake() {
            LoadItems();
            GenerateInventoryItems();
            GenerateFindableItems();
        }

        private void LoadItems() {
            allItems.AddRange(FmvData.GenerateItemDataFromLocalFile(LoadFmvConfig.Config.LocalFilePath).Where((item) => !item.WasUsed));
        }

        private void GenerateInventoryItems() {
            allInventoryItems.AddRange(
                GenerateItems(allItems.Where((item) => item.IsInInventory), inventoryElementsPanel));

            SetEventsForInventoryItems();
        }

        private void SetEventsForInventoryItems() {
            for (int i = 0; i < allFindableItems.Count; i++) {
                SetEventsForInventoryItem(allInventoryItems[i]);
            }
        }

        private void SetEventsForInventoryItem(FmvItemFacade itemFacade) {
            itemFacade.OnItemClicked.RemoveAllListeners();
            itemFacade.OnItemClicked.AddListener(ItemFromInventoryToUsed);
        }

        private void GenerateFindableItems() {
            allFindableItems.AddRange(
                GenerateItems(allItems.Where((item) => !item.IsInInventory && !item.WasUsed), videoElementsPanel));

            SetEventsForFindableItems();
        }

        private void SetEventsForFindableItems() {
            for (int i = 0; i < allFindableItems.Count; i++) {
                SetEventsForFindableItem(allFindableItems[i]);
            }
        }

        private void SetEventsForFindableItem(FmvItemFacade itemFacade) {
            for (int i = 0; i < allFindableItems.Count; i++) {
                itemFacade.OnItemClicked.RemoveAllListeners();
                itemFacade.OnItemClicked.AddListener(ItemFromFindableToInventory);
                itemFacade.OnItemClicked.AddListener(TriggerNaviagtionTarget);
            }
        }

        private List<FmvItemFacade> GenerateItems(IEnumerable<ItemModel> items, RectTransform parent) {
            List<FmvItemFacade> createdItems = new List<FmvItemFacade>();
            foreach (ItemModel item in items) {
                createdItems.Add(CreateItem(item, parent));
            }
            return createdItems;
        }

        private FmvItemFacade CreateItem(ItemModel itemModel, RectTransform parent) {
            FmvItemFacade itemFacade = CreateItemFacade(parent);
            itemFacade.SetItemData(itemModel);
            return itemFacade;
        }

        private FmvItemFacade CreateItemFacade(Transform parentTransform) {
            GameObject targetObject = Instantiate(itemObjectPrefab);
            targetObject.SetActive(true);
            targetObject.transform.SetParent(parentTransform);
            targetObject.transform.localScale = Vector3.one;

            return targetObject.GetComponent<FmvItemFacade>();
        }

        private void AddItemToInventory(ItemModel model) {
            FmvItemFacade itemFacade = CreateItem(model, inventoryElementsPanel);
            SetEventsForInventoryItem(itemFacade);

            allInventoryItems.Add(itemFacade);

            SetItemToIsInInventory(model);
        }

        private void RemoveItemFromItemList(List<FmvItemFacade> items, ItemModel model) {
            FmvItemFacade itemToRemove = items.Find((item) => item.name.Equals(model.Name));
            if (itemToRemove && items.Remove(itemToRemove)) {
                Destroy(itemToRemove.gameObject);
            }
        }

        private void SetItemToIsInInventory(ItemModel model) {
            for (int i = 0; i < allItems.Count; i++) {
                if (allItems[i].Name.Equals(model.Name)) {
                    allItems[i].IsInInventory = true;
                    break;
                }
            }
        }

        private void SetItemToAlreadyUsed(ItemModel model) {
            for (int i = 0; i < allItems.Count; i++) {
                if (allItems[i].Name.Equals(model.Name)) {
                    allItems[i].IsInInventory = false;
                    allItems[i].WasUsed = true;
                    break;
                }
            }
        }

        public void ItemFromFindableToInventory(ItemModel model) {
            RemoveItemFromItemList(allFindableItems, model);
            AddItemToInventory(model);
        }

        private void TriggerNaviagtionTarget(ItemModel model) {
            //throw new NotImplementedException();
        }

        public void ItemFromInventoryToUsed(ItemModel model) {
            if (fmvVideos.CheckCurrentItemsForItemToUse(model)) {
                RemoveItemFromItemList(allInventoryItems, model);
                SetItemToAlreadyUsed(model);
            }
        }
    }
}