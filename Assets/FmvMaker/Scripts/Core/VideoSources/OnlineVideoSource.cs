using FmvMaker.Core.Interfaces;
using FmvMaker.Core.Utilities;
using System;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core.VideoSources {
    public class OnlineVideoSource : MonoBehaviour, IVideoSource {

        private VideoPlayer videoPlayer;

        private void Awake() {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        public void SetVideoSource(string videoName) {
            string elementUri = ResourceVideoInfo.LoadVideoClipFromOnlineSource(videoName);
            if (Uri.TryCreate(elementUri, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) {
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = elementUri;
            } else {
                Debug.LogError($"Video file {elementUri} could not be loaded from online source.");
            }
        }
    }
}