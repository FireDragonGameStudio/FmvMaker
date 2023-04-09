using FmvMaker.Core.Interfaces;
using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using FmvMaker.Core.VideoSources;
using System;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core.Facades {
    public class FmvVideoFacade : MonoBehaviour, IVideoEvents {

        public event Action OnPreparationCompleted;
        public event Action<VideoModel> OnPlayerStarted;
        public event Action<VideoModel> OnLoopPointReached;

        private VideoPlayer videoPlayer;
        private AudioSource audioSource;
        private VideoModel videoModel;
        private IVideoSource videoSource;

        public bool IsLooping => videoPlayer.isLooping;

        public bool IsPlaying => videoPlayer.isPlaying;

        private void Awake() {
            videoPlayer = GetComponent<VideoPlayer>();
            audioSource = GetComponent<AudioSource>();

            ReleaseRenderTexture();
            SetupVideoPlayerConfig();
            SetupUnityVideoEvents();
        }

        private void OnDestroy() {
            DisposeUnityVideoEvents();
            ReleaseRenderTexture();
        }

        private void ReleaseRenderTexture() {
            videoPlayer.targetTexture.Release();
        }

        private void SetupVideoPlayerConfig() {
            switch (LoadFmvConfig.Config.VideoSourceType) {
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
            videoPlayer.started += PlayerStarted;
            videoPlayer.prepareCompleted += PreparationComplete;
            videoPlayer.loopPointReached += LoopPointReached;
        }

        private void DisposeUnityVideoEvents() {
            videoPlayer.started -= PlayerStarted;
            videoPlayer.prepareCompleted -= PreparationComplete;
            videoPlayer.loopPointReached -= LoopPointReached;
        }

        private void PlayerStarted(VideoPlayer source) {
            OnPlayerStarted?.Invoke(videoModel);
        }

        private void PreparationComplete(VideoPlayer source) {
            OnPreparationCompleted?.Invoke();
        }

        private void LoopPointReached(VideoPlayer source) {
            OnLoopPointReached?.Invoke(videoModel);
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
            videoModel = videoElement;
            videoSource.SetVideoSource(videoElement.VideoTarget);
            videoPlayer.isLooping = videoElement.IsLooping;
            videoPlayer.Prepare();
        }

        public void Skip() {
            if (!videoPlayer.isLooping && videoPlayer.isPlaying) {
                videoPlayer.frame = (long)videoPlayer.frameCount - 5;
            } else {
                LoopPointReached(videoPlayer);
            }
        }
    }
}