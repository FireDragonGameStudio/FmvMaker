using FmvMaker.Core.Interfaces;
using FmvMaker.Core.VideoSources;
using FmvMaker.Models;
using FmvMaker.Core.Utilities;
using System;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core.Facades {
    public class VideoFacade : MonoBehaviour, IVideoEvents {

        public event Action OnPlayerStarted;
        public event Action OnLoopPointReached;
        public event Action OnPreparationCompleted;

        private VideoPlayer videoPlayer;
        private AudioSource audioSource;
        private IVideoSource videoSource;

        public bool IsLooping {
            get {
                return videoPlayer.isLooping;
            }
            set {
                _ = videoPlayer.isLooping;
            }
        }

        public bool IsPlaying => videoPlayer.isPlaying;

        private void Awake() {
            videoPlayer = GetComponent<VideoPlayer>();
            audioSource = GetComponent<AudioSource>();

            SetupVideoPlayerConfig();
            SetupUnityVideoEvents();
        }

        private void OnDestroy() {
            DisposeUnityVideoEvents();
        }

        private void SetupVideoPlayerConfig() {
            switch (LoadFmvConfig.Config.SourceType) {
                case "LOCAL":
                    videoSource = gameObject.AddComponent<LocalVideoSource>();
                    break;
                case "ONLINE":
                    videoSource = gameObject.AddComponent<OnlineVideoSource>();
                    break;
                default:
                    videoSource = gameObject.AddComponent<InternalVideoSource>();
                    break;
            }
        }

        private void SetupUnityVideoEvents() {
            videoPlayer.loopPointReached += LoopPointReached;
            videoPlayer.started += PlayerStarted;
            videoPlayer.prepareCompleted += PreparationComplete;
        }

        private void DisposeUnityVideoEvents() {
            videoPlayer.loopPointReached -= LoopPointReached;
            videoPlayer.started -= PlayerStarted;
            videoPlayer.prepareCompleted -= PreparationComplete;
        }

        private void LoopPointReached(VideoPlayer source) {
            OnLoopPointReached?.Invoke();
        }

        private void PlayerStarted(VideoPlayer source) {
            OnPlayerStarted?.Invoke();
        }

        private void PreparationComplete(VideoPlayer source) {
            OnPreparationCompleted?.Invoke();
        }

        public void Play() {
            videoPlayer.Play();
            audioSource.Play();
        }

        public void Pause() {
            videoPlayer.Pause();
            audioSource.Pause();
        }

        public void Stop() {
            videoPlayer.Stop();
            audioSource.Stop();
        }

        public void Prepare(VideoModel videoElement) {
            videoSource.SetVideoSource(videoElement.Name);
            videoPlayer.isLooping = videoElement.IsLooping;
            videoPlayer.Prepare();
        }

        public void SkipVideoClip() {
            if (!videoPlayer.isLooping && videoPlayer.isPlaying) {
                videoPlayer.frame = (long)videoPlayer.frameCount - 5;
            } else {
                LoopPointReached(videoPlayer);
            }
        }
    }
}