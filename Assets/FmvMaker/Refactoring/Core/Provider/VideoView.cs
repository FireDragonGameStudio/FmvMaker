using FmvMaker.Core.Facades;
using FmvMaker.Core.Interfaces;
using FmvMaker.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class VideoView : MonoBehaviour, IVideoEvents {

        public event Action OnPlayerStarted;
        public event Action OnLoopPointReached;
        public event Action OnPreparationCompleted;

        [SerializeField]
        private VideoFacade firstPlayer;
        [SerializeField]
        private VideoFacade secondPlayer;

        // false -> select second player, true -> select first player
        private bool videoPlayerToggle = true;
        private VideoFacade inactivePlayer;

        public VideoFacade ActivePlayer => GetActivePlayer();

        private void Awake() {
            SetupVideoFacadeEvents();
        }

        private void OnDestroy() {
            DisposeVideoFacadeEvents();
        }

        public void SkipVideoClip() {
            ActivePlayer.SkipVideoClip();
        }

        public void PrepareAndPlayVideoClip(VideoModel videoModel) {
            inactivePlayer.Prepare(videoModel);
        }

        private void SetupVideoFacadeEvents() {
            firstPlayer.OnLoopPointReached += LoopPointReached;
            firstPlayer.OnPlayerStarted += PlayerStarted;
            firstPlayer.OnPreparationCompleted += PreparationComplete;

            secondPlayer.OnLoopPointReached += LoopPointReached;
            secondPlayer.OnPlayerStarted += PlayerStarted;
            secondPlayer.OnPreparationCompleted += PreparationComplete;
        }

        private void DisposeVideoFacadeEvents() {
            firstPlayer.OnLoopPointReached -= LoopPointReached;
            firstPlayer.OnPlayerStarted -= PlayerStarted;
            firstPlayer.OnPreparationCompleted -= PreparationComplete;

            secondPlayer.OnLoopPointReached -= LoopPointReached;
            secondPlayer.OnPlayerStarted -= PlayerStarted;
            secondPlayer.OnPreparationCompleted -= PreparationComplete;
        }

        private void LoopPointReached() {
            OnLoopPointReached?.Invoke();
        }

        private async void PlayerStarted() {
            // wait a short time for smoother blending
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            inactivePlayer.Stop();
            OnPlayerStarted?.Invoke();
        }

        private void PreparationComplete() {
            ActivePlayer.Play();

            // show nav elements immediately on looping videos
            if (ActivePlayer.IsLooping) {
                OnLoopPointReached.Invoke();
            }

            videoPlayerToggle = !videoPlayerToggle;
            OnPreparationCompleted?.Invoke();
        }

        private VideoFacade GetActivePlayer() {
            if (videoPlayerToggle) {
                inactivePlayer = secondPlayer;
                return firstPlayer;
            }
            inactivePlayer = firstPlayer;
            return secondPlayer;
        }
    }
}