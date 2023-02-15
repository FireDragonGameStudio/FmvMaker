using FmvMaker.Graph;
using System;
using UnityEngine;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class SaveGameModel {
        public string Id;
        public FmvVideoEnum VideoTarget;
        public FmvVideoEnum UsageTarget;
        public bool IsLooping;
        public bool IsItem;
        public bool IsInInventory;
        public bool WasUsed;
        public Vector2 RelativeScreenPosition = new Vector2(0.5f, 0.5f);
    }
}