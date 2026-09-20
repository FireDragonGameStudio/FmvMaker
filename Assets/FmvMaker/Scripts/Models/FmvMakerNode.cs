using FmvMaker.Provider;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Models {
    [Serializable]
    public class FmvMakerNode {
        public Hash128 NodeId;
        public string NodeName;
        public VideoClip VideoClip;
        public bool IsLooping;
        public bool IsTimedInteraction;
        public FmvInventoryItem NeededItem;
        public FmvInventoryItem GivingItem;
        public bool HasDecisionData;
        public List<FmvMakerDecisionData> DecisionData = new();
        public Hash128 NextNodeId;
        public Hash128 InputPortId;
        public Hash128 OutputPortId;
    }
}