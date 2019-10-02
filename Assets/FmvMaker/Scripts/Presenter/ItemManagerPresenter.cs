using FmvMaker.Models;
using FmvMaker.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FmvMaker.Presenter {
    public class ItemManagerPresenter : MonoBehaviour {

        [SerializeField]
        private RectTransform _inventoryElementsPanel = null;

        private List<ItemModel> _allItemElements;

        private void Start() {
            _allItemElements = FmvData.GenerateItemDataFromLocalFile(LoadFmvConfig.Config.LocalFilePath);
            GenerateItemElements();
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.X)) {
                FmvData.ExportItemDataToLocalFile(_allItemElements, LoadFmvConfig.Config.LocalFilePath);
            }
        }

        private void GenerateItemElements() {
            foreach (ItemModel currentItem in _allItemElements) {
                if (currentItem.IsInInventory && !currentItem.WasUsed) {
                    GameObject itemObject = ObjectPool.Instance.GetPooledItemObject();
                    itemObject.SetActive(true);
                    itemObject.transform.SetParent(_inventoryElementsPanel.transform);
                    itemObject.transform.localScale = Vector3.one;

                    //ItemView view = itemObject.GetComponent<ItemView>();
                    //view.SetItemData(currentItem);
                    //view.OnItemClicked.AddListener(() => {
                    //    view.AddToInventory(_inventoryElementsPanel.transform);
                    //    currentItem.IsInInventory = true;
                    //    view.OnItemClicked.RemoveAllListeners();
                    //    view.OnItemClicked.AddListener(() => {
                    //        currentItem.WasUsed = true;
                    //        ObjectPool.Instance.RemoveItemObjectFromPool(itemObject);
                    //        itemObject.transform.SetParent(_videoElementsPanel.transform);
                    //        OnNavigationClicked(currentItem.NavigationTarget);
                    //    });
                    //});
                }
            }
        }
    }
}
