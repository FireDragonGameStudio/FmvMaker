using UnityEngine;

namespace FmvMaker.Core.Interfaces {
    public interface ICountdownEvents {
        void InitCountdown(float countdownTime);
        void ResetCountdown();
    }
}