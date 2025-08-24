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
        [SerializeField]
        private FmvVideoFacade firstPlayer = null;
        [SerializeField]
        private FmvVideoFacade secondPlayer = null;

        // false -> select second player, true -> select first player
        private bool videoPlayerToggle = true;

        private FmvVideoFacade inactivePlayer;

        public FmvVideoFacade ActivePlayer => GetActivePlayer();

        private bool isActivePlayerLooping;
        private bool loopingPlayerToggle;

        private void Awake() {
            SetupVideoFacadeEvents();
        }

        private void OnDestroy() {
            DisposeVideoFacadeEvents();
        }

        public void StopVideoClip() {
            ActivePlayer.Stop();
        }

        public void SkipVideoClip(VideoClip video) {
            ActivePlayer.Skip();
            OnVideoSkipped?.Invoke(video);
        }

        public void PauseVideoClip(VideoClip video) {
            ActivePlayer.Pause();
            OnVideoPaused?.Invoke(video, !ActivePlayer.IsPlaying);
        }

        public void ResumeVideoClip(VideoClip video) {
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

        private async void PlayerStarted(VideoClip video) {
            // wait a short time for smoother blending
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            inactivePlayer.Stop();
            OnVideoStarted?.Invoke(video);

            isActivePlayerLooping = ActivePlayer.IsLooping;
            loopingPlayerToggle = false;
        }

        private void LoopPointReached(VideoClip video) {
            // don't fire OnVideoFinished for every loop of looping video
            // and don't toggle player every time looping video is finished
            if (isActivePlayerLooping) {
                if (!loopingPlayerToggle) {
                    loopingPlayerToggle = true;
                    videoPlayerToggle = !videoPlayerToggle;
                }
                return;
            }

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