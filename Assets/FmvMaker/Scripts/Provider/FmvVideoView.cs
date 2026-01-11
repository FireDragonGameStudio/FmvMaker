using FmvMaker.Core.Facades;
using FmvMaker.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Provider {
    public class FmvVideoView : MonoBehaviour {

        public event Action<VideoClip> OnVideoStarted;
        public event Action<VideoClip, bool> OnVideoPaused;
        public event Action<VideoClip> OnVideoSkipped;
        public event Action<VideoClip> OnVideoFinished;

        [Header("Internal references")]
        [SerializeField] private FmvVideoFacade firstPlayer = null;
        [SerializeField] private FmvVideoFacade secondPlayer = null;

        // false -> select second player, true -> select first player
        private bool videoPlayerToggle = false;

        private FmvVideoFacade inactivePlayer;

        public FmvVideoFacade ActivePlayer => GetActivePlayer();

        private void Awake() {
            SetupVideoFacadeEvents();
        }

        private void OnDestroy() {
            DisposeVideoFacadeEvents();
        }

        public void StopVideoClip() {
            ActivePlayer.Stop();
        }

        public void SkipVideoClip() {
            ActivePlayer.Skip();
            OnVideoSkipped?.Invoke(ActivePlayer.VideoClip);
        }

        public void PauseVideoClip() {
            ActivePlayer.Pause();
            OnVideoPaused?.Invoke(ActivePlayer.VideoClip, !ActivePlayer.IsPlaying);
        }

        public void ResumeVideoClip() {
            ActivePlayer.Play();
            OnVideoPaused?.Invoke(ActivePlayer.VideoClip, !ActivePlayer.IsPlaying);
        }

        public void PrepareAndPlay(VideoModel videoModel) {
            videoPlayerToggle = !videoPlayerToggle;
            ActivePlayer.Prepare(videoModel);
            inactivePlayer.Pause();
        }

        private void SetupVideoFacadeEvents() {
            firstPlayer.OnPlayerStarted += PlayerStarted;
            firstPlayer.OnPreparationCompleted += PreparationComplete;
            firstPlayer.OnLoopPointReached += LoopPointReached;

            secondPlayer.OnPlayerStarted += PlayerStarted;
            secondPlayer.OnPreparationCompleted += PreparationComplete;
            secondPlayer.OnLoopPointReached += LoopPointReached;
        }

        private void DisposeVideoFacadeEvents() {
            firstPlayer.OnPlayerStarted -= PlayerStarted;
            firstPlayer.OnPreparationCompleted -= PreparationComplete;
            firstPlayer.OnLoopPointReached -= LoopPointReached;

            secondPlayer.OnPlayerStarted -= PlayerStarted;
            secondPlayer.OnPreparationCompleted -= PreparationComplete;
            secondPlayer.OnLoopPointReached -= LoopPointReached;
        }

        private void PreparationComplete() {
            ActivePlayer.Play();
        }

        private async void PlayerStarted(VideoClip video) {
            // wait a short time for smoother blending
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            inactivePlayer.Stop();
            OnVideoStarted?.Invoke(video);
        }

        private void LoopPointReached(VideoClip video) {
            OnVideoFinished.Invoke(video);
        }

        private FmvVideoFacade GetActivePlayer() {
            if (videoPlayerToggle) {
                inactivePlayer = secondPlayer;
                return firstPlayer;
            }
            inactivePlayer = firstPlayer;
            return secondPlayer;
        }
    }
}