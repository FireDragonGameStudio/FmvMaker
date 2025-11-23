using FmvMaker.Provider;
using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Core.Facades {
    public class FmvInventory : MonoBehaviour {
        [SerializeField] private List<FmvInventoryItem> items = new List<FmvInventoryItem>();

        public bool ContainsItem(FmvInventoryItem item) {
            return items.Contains(item);
        }

        public void AddEntry(FmvInventoryItem entry) {
            items.Add(entry);
        }

        public void RemoveEntry(FmvInventoryItem entry) {
            items.Remove(entry);
        }
    }
}