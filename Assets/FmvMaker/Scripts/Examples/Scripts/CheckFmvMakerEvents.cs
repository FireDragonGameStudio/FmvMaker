using FmvMaker.Core.Models;
using FmvMaker.Utilities.Interfaces;
using UnityEngine;

namespace FmvMaker.Examples.Scripts {
    public class CheckFmvMakerEvents : MonoBehaviour, IFmvMakerVideoEvents {
        public void OnVideoFinished(VideoModel videoModel) {
            Debug.Log($"CustomEvent: Video {videoModel.Name} finished.");
        }

        public void OnVideoPaused(VideoModel videoModel, bool isPaused) {
            Debug.Log($"CustomEvent: Video {videoModel.Name} paused. Pause: {isPaused}");
        }

        public void OnVideoSkipped(VideoModel videoModel) {
            Debug.Log($"CustomEvent: Video {videoModel.Name} skipped.");
        }

        public void OnVideoStarted(VideoModel videoModel) {
            Debug.Log($"CustomEvent: Video {videoModel.Name} started. Looping: {videoModel.IsLooping}");
        }
    }
}
