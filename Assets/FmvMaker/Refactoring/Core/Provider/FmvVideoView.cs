using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvVideoView : MonoBehaviour {

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
            GetActivePlayer();
        }

        private void OnDestroy() {
            DisposeVideoFacadeEvents();
        }

        public void SkipVideoClip() {
            ActivePlayer.Skip();
        }

        public void PauseVideoClip() {
            ActivePlayer.Pause();
        }

        public void PrepareAndPlay(VideoModel videoModel) {
            ActivePlayer.Prepare(videoModel);
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
            videoPlayerToggle = !videoPlayerToggle;
        }

        private async void PlayerStarted(VideoModel videoModel) {
            // wait a short time for smoother blending
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            inactivePlayer.Stop();
            PlayerStarted(videoModel);
        }

        private void LoopPointReached(VideoModel videoModel) { }

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