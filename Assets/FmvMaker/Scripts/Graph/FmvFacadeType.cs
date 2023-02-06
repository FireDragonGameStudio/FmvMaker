using FmvMaker.Core.Interfaces;
using FmvMaker.Core.Models;
using System;
using Unity.VisualScripting;
using UnityEngine.Video;

namespace FmvMaker.Graph {
    [Inspectable]
    public class FmvFacadeType {

        public event Action OnPreparationCompleted;
        public event Action<VideoModel> OnPlayerStarted;
        public event Action<VideoModel> OnLoopPointReached;

        [Inspectable]
        private VideoPlayer videoPlayer;
        //[Inspectable]
        //private AudioSource audioSource;

        [Inspectable]
        public bool IsLooping => videoPlayer.isLooping;
        [Inspectable]
        public bool IsPlaying => videoPlayer.isPlaying;

        private VideoModel videoModel;
        private IVideoSource videoSource;

        public FmvFacadeType() {
            ReleaseRenderTexture();
            SetupUnityVideoEvents();
        }

        ~FmvFacadeType() {
            DisposeUnityVideoEvents();
            ReleaseRenderTexture();
        }

        private void SetupUnityVideoEvents() {
            //videoPlayer.started += PlayerStarted;
            //videoPlayer.prepareCompleted += PreparationComplete;
            //videoPlayer.loopPointReached += LoopPointReached;
        }

        private void ReleaseRenderTexture() {
            //videoPlayer.targetTexture.Release();
        }

        private void DisposeUnityVideoEvents() {
            //videoPlayer.started -= PlayerStarted;
            //videoPlayer.prepareCompleted -= PreparationComplete;
            //videoPlayer.loopPointReached -= LoopPointReached;
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
            //videoPlayer.Play();
            //audioSource.Play();
        }

        public void Pause() {
            //videoPlayer.Pause();
            //audioSource.Pause();
        }

        public void Stop() {
            //videoPlayer.Stop();
            //audioSource.Stop();
        }

        public void Prepare(VideoModel videoElement) {
            videoModel = videoElement;
            videoSource.SetVideoSource(videoElement.Name);
            //videoPlayer.isLooping = videoElement.IsLooping;
            //videoPlayer.Prepare();
        }

        public void Skip() {
            //if (!videoPlayer.isLooping && videoPlayer.isPlaying) {
            //    videoPlayer.frame = (long)videoPlayer.frameCount - 5;
            //} else {
            //    LoopPointReached(videoPlayer);
            //}
        }
    }
}