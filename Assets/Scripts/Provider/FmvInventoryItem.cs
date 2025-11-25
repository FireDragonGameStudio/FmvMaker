using System;
using UnityEngine;

namespace FmvMaker.Provider {
    [CreateAssetMenu(menuName = "FmvMaker/InventoryItem")]
    [Serializable]
    public class FmvInventoryItem : ScriptableObject {
        public string Id;        // Internal Id
        public string DisplayName;
        public Sprite DisplayImage;
        public bool MultiUse;
        public bool WasUsed;
    }
}