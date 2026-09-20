using FmvMaker.Provider;
using System;
using UnityEngine;

namespace FmvMaker.Models {
    [Serializable]
    public class FmvMakerDecisionData {
        public string DecisionText;
        public Hash128 DestinationId;
        public string LabelText;
        public Color LabelTextColor;
        public Vector2 RelativePosition;
        public Vector2 RelativeSize;
    }
}