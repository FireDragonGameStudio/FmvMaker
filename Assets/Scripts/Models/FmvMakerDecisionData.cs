using FmvMaker.Provider;
using System;
using UnityEngine;

namespace FmvMaker.Models {
    [Serializable]
    public class FmvMakerDecisionData {
        public string DecisionText;
        public string DestinationId;
        public Vector2 RelativePosition;
        public Vector2 RelativeSize;
    }
}