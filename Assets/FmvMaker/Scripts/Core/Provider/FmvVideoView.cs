using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvVideoView : MonoBehaviour {

        public event Action<VideoModel> OnVideoStarted;
        public event Action<VideoModel, bool> OnVideoPaused;
        public event Action<VideoModel> OnVideoSkipped;
        public event Action<VideoModel> OnVideoFinished;

        [Header("Internal references")]
        [SerializeField]
        private FmvVideoFacade firstPlayer = null;
        [SerializeField]
        private FmvVideoFacade secondPlayer = null;

        // false -> select second player, true -> select first player
        private bool videoPlayerToggle = true;

        private FmvVideoFacade inactivePlayer;

        public FmvVideoFacade ActivePlayer => GetActivePlayer();

        private void Awake() {
            SetupVideoFacadeEvents();
        }

        private void OnDestroy() {
            DisposeVideoFacadeEvents();
        }

        public void SkipVideoClip(VideoModel video) {
            ActivePlayer.Skip();
            OnVideoSkipped?.Invoke(video);
        }

        public void PauseVideoClip(VideoModel video) {
            ActivePlayer.Pause();
            OnVideoPaused?.Invoke(video, !ActivePlayer.IsPlaying);
        }

        public void ResumeVideoClip(VideoModel video) {
            ActivePlayer.Play();
            OnVideoPaused?.Invoke(video, !ActivePlayer.IsPlaying);
        }

        public void PrepareAndPlay(VideoModel videoModel) {
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

        private async void PlayerStarted(VideoModel video) {
            // wait a short time for smoother blending
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            inactivePlayer.Stop();
            OnVideoStarted?.Invoke(video);
        }

        private void LoopPointReached(VideoModel video) {
            videoPlayerToggle = !videoPlayerToggle;
            OnVideoFinished(video);
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