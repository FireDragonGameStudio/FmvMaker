using System;
using UnityEngine.Video;

namespace FmvMaker.Core.Interfaces {
    public interface IVideoEvents {
        event Action OnPreparationCompleted;
        event Action<VideoClip> OnPlayerStarted;
        event Action<VideoClip> OnLoopPointReached;
    }
}