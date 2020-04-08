using FmvMaker.Models;
using FmvMaker.Core.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Views {
    public class VideoView : MonoBehaviour {

        public event Action OnPlayerStarted;
        public event Action OnLoopPointReached;
        public event Action OnPreparationCompleted;

        [SerializeField]
        private VideoPlayer _firstPlayer;
        [SerializeField]
        private VideoPlayer _secondPlayer;

        private AudioSource _firstAudioSource;
        private AudioSource _secondAudioSource;

        private VideoPlayer _activeVideoPlayer;
        private VideoPlayer _inactiveVideoPlayer;

        // false -> select second player, true -> select first player
        private bool _videoPlayerToggle = true;

        public bool IsLooping => _activeVideoPlayer.isLooping;
        public bool IsPlaying => _activeVideoPlayer.isPlaying;

        private void Awake() {
            if (!_firstPlayer || !_secondPlayer) {
                Debug.LogWarning("No video players/audio sources for view set. I'll try to find it automatically. Please check if references are set, before starting playmode again.", this);
                _firstPlayer = transform.GetChild(0).GetComponent<VideoPlayer>();
                _secondPlayer = transform.GetChild(1).GetComponent<VideoPlayer>();
            }

            _firstAudioSource = _firstPlayer.GetComponent<AudioSource>();
            _secondAudioSource = _secondPlayer.GetComponent<AudioSource>();

            SetupVideoEvents();

            _activeVideoPlayer = _secondPlayer;
            _inactiveVideoPlayer = _firstPlayer;
        }

        private void OnDestroy() {
            _firstPlayer.loopPointReached -= LoopPointReached;
            _firstPlayer.started -= PlayerStarted;
            _firstPlayer.prepareCompleted -= PreparationComplete;

            _secondPlayer.loopPointReached -= LoopPointReached;
            _secondPlayer.started -= PlayerStarted;
            _secondPlayer.prepareCompleted -= PreparationComplete;
        }

        private void SetupVideoEvents() {
            _firstPlayer.loopPointReached += LoopPointReached;
            _firstPlayer.started += PlayerStarted;
            _firstPlayer.prepareCompleted += PreparationComplete;

            _secondPlayer.loopPointReached += LoopPointReached;
            _secondPlayer.started += PlayerStarted;
            _secondPlayer.prepareCompleted += PreparationComplete;
        }

        private void LoopPointReached(VideoPlayer source) {
            OnLoopPointReached?.Invoke();
        }

        private async void PlayerStarted(VideoPlayer source) {
            // wait a short time for smoother blending
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            _inactiveVideoPlayer.Stop();
            OnPlayerStarted?.Invoke();
        }

        private void PreparationComplete(VideoPlayer source) {
            // play actual player and set active/inactive players
            if (_videoPlayerToggle) {
                _firstPlayer.Play();
                _firstAudioSource.Play();
                _activeVideoPlayer = _firstPlayer;
                _inactiveVideoPlayer = _secondPlayer;
            } else {
                _secondPlayer.Play();
                _secondAudioSource.Play();
                _activeVideoPlayer = _secondPlayer;
                _inactiveVideoPlayer = _firstPlayer;
            }

            // show nav elements immediately on looping videos
            if (_activeVideoPlayer.isLooping) {
                OnLoopPointReached.Invoke();
            }

            _videoPlayerToggle = !_videoPlayerToggle;

            OnPreparationCompleted?.Invoke();
        }

        public void PrepareAndPlayVideoClip(VideoModel videoElement) {
            if (LoadFmvConfig.Config.SourceType.Equals("LOCAL")) {
                _inactiveVideoPlayer.source = VideoSource.Url;
                _inactiveVideoPlayer.url = LoadVideoFromLocalFile(videoElement.Name);
            } else if (LoadFmvConfig.Config.SourceType.Equals("ONLINE")) {
                string elementUri = LoadVideoFromLocalFile(videoElement.Name);
                if (Uri.TryCreate(elementUri, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) {
                    _inactiveVideoPlayer.source = VideoSource.Url;
                    _inactiveVideoPlayer.url = LoadVideoFromOnlineSource(videoElement.Name);
                } else {
                    Debug.LogError($"Video file {elementUri} could not be loaded.");
                }
            } else {
                _inactiveVideoPlayer.source = VideoSource.VideoClip;
                _inactiveVideoPlayer.clip = LoadVideoFromResources(videoElement.Name);
            }
            _inactiveVideoPlayer.isLooping = videoElement.IsLooping;
            _inactiveVideoPlayer.Prepare();
        }

        public void SkipVideoClip() {
            if (!_activeVideoPlayer.isLooping && _activeVideoPlayer.isPlaying) {
                _activeVideoPlayer.frame = (long)_activeVideoPlayer.frameCount - 5;
            } else {
                OnLoopPointReached.Invoke();
            }
        }

        private VideoClip LoadVideoFromResources(string name) {
            return ResourceInfo.LoadVideoClipFromResources(name);
        }

        private string LoadVideoFromLocalFile(string name) {
            return ResourceInfo.LoadVideoClipFromFile(name);
        }

        private string LoadVideoFromOnlineSource(string name) {
            return ResourceInfo.LoadVideoClipFromOnlineSource(name);
        }
    }
}