using System;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Models {
    [Serializable]
    public class VideoModel {
        public Hash128 NodeId;
        public string NodeName;
        public VideoClip VideoClip;
        public bool IsLooping;
    }
}