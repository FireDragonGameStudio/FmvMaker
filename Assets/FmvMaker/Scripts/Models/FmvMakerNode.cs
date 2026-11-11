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
        // plays no video, picks one of its DecisionData entries by Weight
        public bool IsRandomizer;
        public List<FmvMakerDecisionData> DecisionData = new();
        public Hash128 NextNodeId;
        // IDs of this node's own ports (not the connected ones), used for wire visualization
        public Hash128 InputPortId;
        public Hash128 OutputPortId;

        /// <summary>Chance (0-1) of a randomizer outcome, relative to the sum of all positive weights.</summary>
        public float GetChance(FmvMakerDecisionData outcome) {
            var total = 0f;
            foreach (var decision in DecisionData) {
                total += Mathf.Max(0f, decision.Weight);
            }
            return total > 0f ? Mathf.Max(0f, outcome.Weight) / total : 0f;
        }
    }
}