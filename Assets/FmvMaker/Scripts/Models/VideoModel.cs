using System;
using UnityEngine.Video;

namespace FmvMaker.Models {
    [Serializable]
    public class VideoModel {
        public string NodeId;
        public string NodeName;
        public VideoClip VideoClip;
        public bool IsLooping;
    }
}