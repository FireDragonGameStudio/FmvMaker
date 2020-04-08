using System;

namespace FmvMaker.Core.Interfaces {
    public interface IVideoEvents {
        event Action OnPlayerStarted;
        event Action OnLoopPointReached;
        event Action OnPreparationCompleted;
    }
}