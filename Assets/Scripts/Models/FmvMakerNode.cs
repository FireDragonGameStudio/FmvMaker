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
        public List<FmvMakerDecisionData> DecisionData = new();
    }
}