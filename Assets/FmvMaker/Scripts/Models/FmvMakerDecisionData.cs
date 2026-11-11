using System;
using UnityEngine;

namespace FmvMaker.Models {
    [Serializable]
    public class FmvMakerDecisionData {
        public string DecisionText;
        public Hash128 DestinationId;
        // ID of this choice's own "out" port (on the block node), used for wire visualization
        public Hash128 OutputPortId;
        // relative chance, only used by randomizer nodes
        public float Weight;
        public string LabelText;
        public Color LabelTextColor;
        public Vector2 RelativePosition;
        public Vector2 RelativeSize;
    }
}