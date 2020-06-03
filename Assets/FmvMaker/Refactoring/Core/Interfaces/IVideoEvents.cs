using FmvMaker.Core.Models;
using System;

namespace FmvMaker.Core.Interfaces {
    public interface IVideoEvents {
        event Action OnPreparationCompleted;
        event Action<VideoModel> OnPlayerStarted;
        event Action<VideoModel> OnLoopPointReached;
    }
}