using FmvMaker.Core.Interfaces;
using FmvMaker.Core.Utilities;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core.VideoSources {
    public class LocalVideoSource : MonoBehaviour, IVideoSource {

        private VideoPlayer videoPlayer;

        private void Awake() {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        public void SetVideoSource(string videoName) {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = ResourceVideoInfo.LoadVideoClipFromFile(videoName);
        }
    }
}