using FmvMaker.Core.Interfaces;
using FmvMaker.Models;
using System;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core.Facades {
    public class FmvVideoFacade : MonoBehaviour, IVideoEvents {

        public event Action OnPreparationCompleted;
        public event Action<VideoClip> OnPlayerStarted;
        public event Action<VideoClip> OnLoopPointReached;

        private VideoPlayer videoPlayer;
        private AudioSource audioSource;

        public bool IsLooping => videoPlayer.isLooping;
        public bool IsPlaying => videoPlayer.isPlaying;
        public VideoClip VideoClip => videoPlayer.clip;
        public AudioSource AudioClip => videoPlayer.GetTargetAudioSource(0);

        private void Awake() {
            videoPlayer = GetComponent<VideoPlayer>();
            audioSource = GetComponent<AudioSource>();

            ReleaseRenderTexture();
            SetupUnityVideoEvents();
        }

        private void OnDestroy() {
            DisposeUnityVideoEvents();
            ReleaseRenderTexture();
        }

        private void ReleaseRenderTexture() {
            videoPlayer.targetTexture.Release();
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
            OnPlayerStarted?.Invoke(source.clip);
        }

        private void PreparationComplete(VideoPlayer sourc) {
            OnPreparationCompleted?.Invoke();
        }

        private void LoopPointReached(VideoPlayer source) {
            OnLoopPointReached?.Invoke(source.clip);
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
            videoPlayer.isLooping = false;
        }

        public void Prepare(VideoModel videoModel) {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoModel.VideoClip;
            videoPlayer.isLooping = videoModel.IsLooping;
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