using FmvMaker.Models;
using FmvMaker.Presenter;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Views {
    // Place me on a ScreenSpace UI canvas gameObject for placing navigation buttons.
    [RequireComponent(typeof(Canvas))]
    public class VideoView : MonoBehaviour {

        [SerializeField]
        private VideoPresenter _presenter;

        void Awake() {
            if (!_presenter) {
                Debug.LogError("No presenter for view set. I'll try to find it automatically. Please check if presenter is set, before starting playmode again.", this);
                _presenter = FindObjectOfType<VideoPresenter>();
            }
        }

        void Start() {
            _presenter.MainPlayer.loopPointReached += ShowBackground;
        }

        void OnDestroy() {
            _presenter.MainPlayer.loopPointReached -= ShowBackground;
        }

        public void PlayMainVideoClip(VideoClip clip) {
            _presenter.MainPlayer.clip = clip;
            _presenter.MainPlayer.Play();
        }

        public void StopBackgroundVideo() {
            _presenter.BackgroundPlayer.Stop();
        }

        private void ShowBackground(VideoPlayer source) {
            _presenter.SetBackgroundPlayer(); 
        }
    }
}