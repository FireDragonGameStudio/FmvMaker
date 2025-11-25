using FmvMaker.Provider;
using UnityEngine;

namespace FmvMaker.Core.Facades {
    public class FmvInventory : MonoBehaviour {

        public FmvInventoryItemList ItemList;

        [Header("Debug Settings")]
        [SerializeField] private bool resetInventoryOnStart;

        private void Awake() {
            if (resetInventoryOnStart) {
                foreach (var item in ItemList.Items) {
                    item.WasUsed = false;
                }

                ItemList.Items.Clear();
            }
        }

        public bool ContainsItem(FmvInventoryItem item) {
            return ItemList.Items.Find(i => item.Id == i.Id);
        }

        public void AddEntry(FmvInventoryItem entry) {
            ItemList.Items.Add(entry);
        }

        public void RemoveEntry(FmvInventoryItem entry) {
            ItemList.Items.Remove(entry);
        }
    }
}