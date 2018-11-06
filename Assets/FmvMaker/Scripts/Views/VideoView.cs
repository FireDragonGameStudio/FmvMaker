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
        [SerializeField]
        private VideoPlayer _mainPlayer;
        [SerializeField]
        private VideoPlayer _backgroundPlayer;

        void Awake() {
            if (!_presenter) {
                Debug.LogError("No presenter for view set. I'll try to find it automatically. Please check if presenter is set, before starting playmode again.", this);
                _presenter = FindObjectOfType<VideoPresenter>();
            }
        }

        void Start() {
            _mainPlayer.loopPointReached += ShowBackground;
            _mainPlayer.started += DisableBackgroundVideo;
            _backgroundPlayer.started += DisableMainVideo;
        }

        void OnDestroy() {
            _mainPlayer.loopPointReached -= ShowBackground;
            _mainPlayer.started -= DisableBackgroundVideo;
            _backgroundPlayer.started -= DisableMainVideo;
        }

        public void PlayMainVideoClip(VideoClip clip) {
            _mainPlayer.url = "";
            _mainPlayer.clip = clip;
            _mainPlayer.Play();
        }

        public void PlayMainVideoClip(string url) {
            _mainPlayer.clip = null;
            _mainPlayer.url = url;
            _mainPlayer.Play();
        }

        public void StartBackgroundVideo() {
            _backgroundPlayer.Play();
        }

        public void StopBackgroundVideo() {
            _backgroundPlayer.Stop();
        }

        private void ShowBackground(VideoPlayer source) {
            _presenter.SetBackgroundPlayer(); 
        }

        private void DisableMainVideo(VideoPlayer source) {
            SetPlayerVisible(false);
        }

        private void DisableBackgroundVideo(VideoPlayer source) {
            _presenter.DisableBackgroundVideo();
        }

        public void SetPlayerVisible(bool isMainVisible = true) {
            if (isMainVisible) {
                _mainPlayer.transform.localPosition = new Vector3(0, 0, 0.01f);
                _backgroundPlayer.transform.localPosition = new Vector3(0, 0, 0.1f);
            } else {
                _mainPlayer.transform.localPosition = new Vector3(0, 0, 0.1f);
                _backgroundPlayer.transform.localPosition = new Vector3(0, 0, 0.01f);
            }
            //mainPlayer.gameObject.SetActive(isMainVisible);
            //backgroundPlayer.gameObject.SetActive(!isMainVisible);
        }

        public void SetStaticBackgroundInfo(Vector3 localEulerRotation, Material playerMaterial) {
            SetBackgroundPlayerRotationAndMaterial(localEulerRotation, playerMaterial);
            _backgroundPlayer.clip = null;
        }

        public void SetDynamicBackgroundInfo(Vector3 localEulerRotation, Material playerMaterial, VideoClip clip) {
            SetBackgroundPlayerRotationAndMaterial(localEulerRotation, playerMaterial);
            _backgroundPlayer.clip = clip;
            _backgroundPlayer.Prepare();
        }

        private void SetBackgroundPlayerRotationAndMaterial(Vector3 localEulerRotation, Material playerMaterial) {
            _backgroundPlayer.transform.localEulerAngles = localEulerRotation;
            _backgroundPlayer.gameObject.GetComponent<Renderer>().material = playerMaterial;
        }
    }
}