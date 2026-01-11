using System;
using UnityEngine.Events;
using UnityEngine.Video;

namespace FmvMaker.Models {
    [Serializable]
    public class VideoBoolEvent : UnityEvent<VideoClip, bool> {
    }
}