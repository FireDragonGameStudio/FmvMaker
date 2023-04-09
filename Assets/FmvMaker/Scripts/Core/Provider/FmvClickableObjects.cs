using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvClickableObjects : MonoBehaviour {

        [Header("Internal references")]
        [SerializeField]
        private FmvVideos fmvVideos = null;
        [SerializeField]
        private FmvData fmvData = null;
        [SerializeField]
        private RectTransform videoElementsPanel = null;
        [SerializeField]
        private RectTransform inventoryElementsPanel = null;
        [SerializeField]
        private GameObject clickableObjectPrefab = null;

        private List<ClickableModel> allItems = new List<ClickableModel>();
        private List<FmvClickableFacade> allInventoryItems = new List<FmvClickableFacade>();
        private List<FmvClickableFacade> allFindableItems = new List<FmvClickableFacade>();
        private List<FmvClickableFacade> allNavigationItems = new List<FmvClickableFacade>();

        private void Awake() {
            LoadItems();
            GenerateInventoryItems();
            GenerateFindableItems();
            GenerateNavigationTargetItems();
        }

        private void LoadItems() {
            allItems.AddRange(fmvData.GenerateClickableDataFromConfigurationFile().Where((item) => !item.WasUsed));
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

        private void SetEventsForInventoryItem(FmvClickableFacade itemFacade) {
            itemFacade.OnItemClicked.RemoveAllListeners();
            itemFacade.OnItemClicked.AddListener(ItemFromInventoryToUsed);
            itemFacade.ChangeVisibility(1);
        }

        private void GenerateFindableItems() {
            allFindableItems.AddRange(
                GenerateItems(allItems.Where((item) => !item.IsInInventory && !item.WasUsed), videoElementsPanel));

            ConfigureFindableItems();
        }

        private void ConfigureFindableItems() {
            for (int i = 0; i < allFindableItems.Count; i++) {
                SetEventsForFindableItem(allFindableItems[i]);
                SetFindableItemInactive(allFindableItems[i]);
            }
        }

        private void SetEventsForFindableItem(FmvClickableFacade itemFacade) {
            itemFacade.OnItemClicked.RemoveAllListeners();
            itemFacade.OnItemClicked.AddListener(ItemFromFindableToInventory);
            itemFacade.OnItemClicked.AddListener(TriggerPickUpNavigationTarget);
        }

        private void GenerateNavigationTargetItems() {
            allNavigationItems.AddRange(
                GenerateItems(allItems.Where((item) => item.IsNavigation && !item.IsInInventory && !item.WasUsed), videoElementsPanel));

            ConfigureNavigationTargetItems();
        }

        private void ConfigureNavigationTargetItems() {
            for (int i = 0; i < allNavigationItems.Count; i++) {
                SetEventsForNavigationTarget(allNavigationItems[i]);
                SetNavigationTargetInactive(allNavigationItems[i]);
            }
        }

        private void SetEventsForNavigationTarget(FmvClickableFacade itemFacade) {
            itemFacade.OnItemClicked.RemoveAllListeners();
            itemFacade.OnItemClicked.AddListener(TriggerNavigationTarget);
        }

        private void SetNavigationTargetInactive(FmvClickableFacade itemFacade) {
            itemFacade.gameObject.SetActive(false);
        }

        private void TriggerNavigationTarget(ClickableModel model) {
            fmvVideos.PlayVideoFromNavigationTarget(model.PickUpVideo);
        }

        private List<FmvClickableFacade> GenerateItems(IEnumerable<ClickableModel> items, RectTransform parent) {
            List<FmvClickableFacade> createdItems = new List<FmvClickableFacade>();
            foreach (ClickableModel item in items) {
                createdItems.Add(CreateItem(item, parent));
            }
            return createdItems;
        }

        private FmvClickableFacade CreateItem(ClickableModel itemModel, RectTransform parent) {
            FmvClickableFacade itemFacade = CreateItemFacade(parent);
            itemFacade.SetItemData(itemModel);
            return itemFacade;
        }

        private FmvClickableFacade CreateItemFacade(Transform parentTransform) {
            GameObject targetObject = Instantiate(clickableObjectPrefab);
            targetObject.SetActive(true);
            targetObject.transform.SetParent(parentTransform);
            targetObject.transform.localScale = Vector3.one;

            return targetObject.GetComponent<FmvClickableFacade>();
        }

        private void SetFindableItemInactive(FmvClickableFacade itemFacade) {
            itemFacade.gameObject.SetActive(false);
        }

        private void AddItemToInventory(ClickableModel model) {
            FmvClickableFacade itemFacade = CreateItem(model, inventoryElementsPanel);
            SetEventsForInventoryItem(itemFacade);

            allInventoryItems.Add(itemFacade);

            SetItemToIsInInventory(model);
        }

        private void RemoveItemFromItemList(List<FmvClickableFacade> items, ClickableModel model) {
            FmvClickableFacade itemToRemove = items.Find((item) => item.name.Equals(model.Name));
            if (itemToRemove && items.Remove(itemToRemove)) {
                Destroy(itemToRemove.gameObject);
            }
        }

        private void SetItemToIsInInventory(ClickableModel model) {
            for (int i = 0; i < allItems.Count; i++) {
                if (allItems[i].Name.Equals(model.Name)) {
                    allItems[i].IsInInventory = true;
                    break;
                }
            }
        }

        private void SetItemToAlreadyUsed(ClickableModel model) {
            for (int i = 0; i < allItems.Count; i++) {
                if (allItems[i].Name.Equals(model.Name)) {
                    allItems[i].IsInInventory = false;
                    allItems[i].WasUsed = true;
                    break;
                }
            }
        }

        private void ItemFromFindableToInventory(ClickableModel model) {
            RemoveItemFromItemList(allFindableItems, model);
            AddItemToInventory(model);
        }

        private void TriggerPickUpNavigationTarget(ClickableModel model) {
            fmvVideos.PlayVideoFromNavigationTarget(model.PickUpVideo);
        }

        private void TriggerUseageNavigationTarget(ClickableModel model) {
            fmvVideos.PlayVideoFromNavigationTarget(model.UsageVideo);
        }

        private void ItemFromInventoryToUsed(ClickableModel model) {
            if (fmvVideos.CheckCurrentItemsForItemToUse(model)) {
                TriggerUseageNavigationTarget(model);
                RemoveItemFromItemList(allInventoryItems, model);
                SetItemToAlreadyUsed(model);
            }
        }

        public void EnableFindableItems(ClickableModel[] itemModels) {
            for (int i = 0; i < itemModels.Length; i++) {
                allFindableItems
                    .SingleOrDefault(item => item.name.ToLower().Equals(itemModels[i].Name.ToLower()))
                    ?.gameObject.SetActive(true);
            }
        }

        public void DisableFindableItems() {
            allFindableItems.ForEach(item => item.gameObject.SetActive(false));
        }

        public void SetNavigationTargetsActive(ClickableModel[] navigationModels) {
            for (int i = 0; i < navigationModels.Length; i++) {
                allNavigationItems
                    .SingleOrDefault(navigation => navigation.name.ToLower().Equals(navigationModels[i].Name.ToLower()))
                    ?.gameObject.SetActive(true);
            }
        }

        public ClickableModel GetNavigationItemModelByName(string navigationName) {
            ClickableModel navigationItem = allItems.SingleOrDefault(navigation => navigation.Name.ToLower().Equals(navigationName.ToLower()));
            return navigationItem;
        }

        public void DisableNavigationTargets() {
            allNavigationItems.ForEach(navigation => navigation.gameObject.SetActive(false));
        }

        public void ToggleFindableItems(bool isVisible) {
            int alphaValue = isVisible ? 255 : 0;
            allFindableItems.ForEach(item => item.ChangeVisibility(alphaValue));
        }
    }
}