using FmvMaker.Provider;
using System;
using System.Collections.Generic;
using UnityEngine.Video;

namespace FmvMaker.Models {
    [Serializable]
    public class FmvMakerNode {
        public string NodeId;
        public string NodeName;
        public VideoClip VideoClip;
        public bool IsLooping;
        public FmvInventoryItem NeededItem;
        public FmvInventoryItem GivingItem;
        public bool HasDecisionData;
        public List<FmvMakerDecisionData> DecisionData = new();
        public string NextNodeId;
    }
}