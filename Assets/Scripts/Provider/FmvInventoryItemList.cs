using System;
using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Provider {
    [CreateAssetMenu(menuName = "FmvMaker/InventoryItemList")]
    [Serializable]
    public class FmvInventoryItemList : ScriptableObject {
        public List<FmvInventoryItem> Items = new();
    }
}