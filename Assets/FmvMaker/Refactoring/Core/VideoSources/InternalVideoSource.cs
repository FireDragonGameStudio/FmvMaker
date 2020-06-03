using FmvMaker.Core.Interfaces;
using FmvMaker.Core.Utilities;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core.VideoSources {
    public class InternalVideoSource : MonoBehaviour, IVideoSource {

        private VideoPlayer videoPlayer;

        private void Awake() {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        public void SetVideoSource(string videoName) {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = ResourceVideoInfo.LoadVideoClipFromResources(videoName);
        }
    }
}