using FmvMaker.Interfaces;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Examples {
    public class CheckFmvMakerEvents : MonoBehaviour, IFmvMakerVideoEvents {
        public void OnVideoFinished(VideoClip videoClip) {
            Debug.Log($"CustomEvent: Video {videoClip.name} finished.");
        }

        public void OnVideoPaused(VideoClip videoClip, bool isPaused) {
            Debug.Log($"CustomEvent: Video {videoClip.name} paused. Pause: {isPaused}");
        }

        public void OnVideoSkipped(VideoClip videoClip) {
            Debug.Log($"CustomEvent: Video {videoClip.name} skipped.");
        }

        public void OnVideoStarted(VideoClip videoClip) {
            Debug.Log($"CustomEvent: Video {videoClip.name} started.");
        }
    }
}
