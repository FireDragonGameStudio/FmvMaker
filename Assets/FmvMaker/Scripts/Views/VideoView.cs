using FmvMaker.Models;
using FmvMaker.Utils;
using System;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Views {
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoView : MonoBehaviour {

        public event Action OnPlayerStarted;
        public event Action OnLoopPointReached;

        [SerializeField]
        private VideoPlayer _mainPlayer;

        private NavigationModel[] _navigationTargets;

        void Awake() {
            if (!_mainPlayer) {
                Debug.LogError("No video player for view set. I'll try to find it automatically. Please check if presenter is set, before starting playmode again.", this);
                _mainPlayer = GetComponent<VideoPlayer>();
            }
        }

        void Start() {
            _mainPlayer.loopPointReached += LoopPointReached;
            _mainPlayer.started += PlayerStarted;
            //_backgroundPlayer.started += DisableMainVideo;
        }

        void OnDestroy() {
            _mainPlayer.loopPointReached -= LoopPointReached;
            _mainPlayer.started -= PlayerStarted;
        }

        private void LoopPointReached(VideoPlayer source) {
            OnLoopPointReached?.Invoke();
        }

        private void PlayerStarted(VideoPlayer source) {
            OnPlayerStarted?.Invoke();
        }

        public void PlayVideoClip(VideoElement videoElement) {
            _navigationTargets = videoElement.NavigationTargets;

            if (Uri.TryCreate(videoElement.Name, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) {
                _mainPlayer.clip = null;
                _mainPlayer.url = videoElement.Name;
            } else {
                _mainPlayer.clip = LoadVideo(videoElement.Name);
            }
            _mainPlayer.Play();
        }

        private VideoClip LoadVideo(string name) {
            return ResourceInfo.LoadVideoClipFromResources(name);
        }
    }
}