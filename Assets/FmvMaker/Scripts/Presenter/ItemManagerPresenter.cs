using FmvMaker.Models;
using FmvMaker.Core.Utilities;
using FmvMaker.Views;
using UnityEngine;

namespace FmvMaker.Presenter {
    public class ItemManagerPresenter : MonoBehaviour {

        [SerializeField]
        private RectTransform _inventoryElementsPanel = null;
        [SerializeField]
        private FmvMakerPresenter _fmvMakerPresenter = null;

        private bool _inventoryToggle;

        private void Update() {
            if (Input.GetKeyUp(KeyCode.I)) {
                float yPos = _inventoryToggle ? 50 : -50;
                _inventoryElementsPanel.position = new Vector3(
                    _inventoryElementsPanel.position.x, yPos, _inventoryElementsPanel.position.z);
                _inventoryToggle = !_inventoryToggle;
            }
        }

        public void AddItemToInventory(ItemModel itemModel) {
            GameObject itemObject = ObjectPool.Instance.GetPooledInventoryItemObject();
            itemObject.SetActive(true);
            itemObject.transform.SetParent(_inventoryElementsPanel.transform);
            itemObject.transform.localScale = Vector3.one;

            ItemView view = itemObject.GetComponent<ItemView>();
            view.SetItemData(itemModel);
            view.OnItemClicked.AddListener((model) => {
                if (_fmvMakerPresenter.OnItemUsed(model)) {
                    ObjectPool.Instance.RemoveInventoryItemObjectFromPool(itemObject);
                }
            });
        }
    }
}
