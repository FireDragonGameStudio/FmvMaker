using FmvMaker.Models;
using FmvMaker.Utils;
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
        private Transform _firstVideoElement;
        [SerializeField]
        private Transform _secondVideoElemnt;

        private VideoPlayer _firstPlayer;
        private VideoPlayer _secondPlayer;
        private AudioSource _firstAudioSource;
        private AudioSource _secondAudioSource;

        // false -> select second player, true -> select first player
        private bool _videoPlayerToggle = true;

        private void Awake() {
            if (!_firstVideoElement || !_secondVideoElemnt) {
                Debug.LogWarning("No video player for view set. I'll try to find it automatically. Please check if references are set, before starting playmode again.", this);
                _firstVideoElement = transform.GetChild(0);
                _secondVideoElemnt = transform.GetChild(1);
            }

            _firstPlayer = _firstVideoElement.GetComponent<VideoPlayer>();
            _firstAudioSource = _firstVideoElement.GetComponent<AudioSource>();
            _secondPlayer = _secondVideoElemnt.GetComponent<VideoPlayer>();
            _secondAudioSource = _secondVideoElemnt.GetComponent<AudioSource>();
        }

        private void Start() {
            _firstPlayer.loopPointReached += LoopPointReached;
            _firstPlayer.started += PlayerStarted;
            _firstPlayer.prepareCompleted += PreparationComplete;

            _secondPlayer.loopPointReached += LoopPointReached;
            _secondPlayer.started += PlayerStarted;
            _secondPlayer.prepareCompleted += PreparationComplete;
        }

        private void OnDestroy() {
            _firstPlayer.loopPointReached -= LoopPointReached;
            _firstPlayer.started -= PlayerStarted;
            _firstPlayer.prepareCompleted -= PreparationComplete;

            _secondPlayer.loopPointReached -= LoopPointReached;
            _secondPlayer.started -= PlayerStarted;
            _secondPlayer.prepareCompleted -= PreparationComplete;
        }

        private void LoopPointReached(VideoPlayer source) {
            OnLoopPointReached?.Invoke();
        }

        private async void PlayerStarted(VideoPlayer source) {
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            DisableOtherPlayer();
            OnPlayerStarted?.Invoke();
        }

        private void PreparationComplete(VideoPlayer source) {
            StartCurrentPlayer();
        }

        public void PlayVideoClip(VideoElement videoElement) {
            VideoPlayer player = _videoPlayerToggle ? _firstPlayer : _secondPlayer;
            if (Uri.TryCreate(videoElement.Name, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) {
                player.source = VideoSource.Url;
                player.url = videoElement.Name;
            } else {
                player.source = VideoSource.VideoClip;
                player.clip = LoadVideo(videoElement.Name);
            }
            player.isLooping = videoElement.IsLooping;
            player.Prepare();
        }

        public void SkipVideoClip() {
            if (_firstPlayer.isPlaying || !_firstPlayer.isLooping) {
                _firstPlayer.frame = (long)_firstPlayer.frameCount - 5;
            }


            //OnLoopPointReached?.Invoke();
        }

        private void StartCurrentPlayer() {
            if (_videoPlayerToggle) {
                _firstPlayer.Play();
                _firstAudioSource.Play();
            } else {
                _secondPlayer.Play();
                _secondAudioSource.Play();
            }
            _videoPlayerToggle = !_videoPlayerToggle;
        }

        private void DisableOtherPlayer() {
            if (_videoPlayerToggle) {
                _firstPlayer.Stop();
                Debug.Log("Stop first player");
            } else {
                _secondPlayer.Stop();
                Debug.Log("Stop second player");
            }
        }

        private VideoClip LoadVideo(string name) {
            return ResourceInfo.LoadVideoClipFromResources(name);
        }
    }
}