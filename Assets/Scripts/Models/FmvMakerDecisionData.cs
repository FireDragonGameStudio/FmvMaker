using System;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Models {
    [Serializable]
    public class FmvMakerDecisionData {
        public string DecisionText;
        public string DestinationId;
        public VideoClip VideoClip;
        public Vector2 RelativePosition;
        public Vector2 RelativeSize;
    }
}