using FmvMaker.Models;
using FmvMaker.Tools;
using FmvMaker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

// Idea: Crowd clients on LAN can vote for a way to go -> Mirror

namespace FmvMaker.Presenter {
    public class VideoPresenter : MonoBehaviour {

        [SerializeField]
        private VideoPlayer mainPlayer;
        [SerializeField]
        private VideoPlayer backgroundPlayer;

        [SerializeField]
        private VideoView view;
        [SerializeField]
        private Material videoMaterial;
        [SerializeField]
        private Material backgroundMaterial;

        private List<VideoInfo> _allVideoElements;
        private bool _isBackgroundStatic;
        private VideoInfo _currentInfo;

        private Vector3 _staticLocalRotationEuler = new Vector3(90, 180, 0);
        private Vector3 _dynamicLocalRotationEuler = new Vector3(180, -90, 90);

        public VideoPlayer MainPlayer {
            get {
                return mainPlayer;
            }
        }

        public VideoPlayer BackgroundPlayer {
            get {
                return backgroundPlayer;
            }
        }

        void Awake() {
            if (!view) {
                Debug.LogError("No view for presenter set. I'll try to find it automatically. Please check if view is set, before starting playmode again.", this);
                view = FindObjectOfType<VideoView>();
            }
        }

        void Start() {
            mainPlayer.gameObject.GetComponent<Renderer>().material = videoMaterial;
            backgroundPlayer.gameObject.GetComponent<Renderer>().material = backgroundMaterial;

            _allVideoElements = LoadFmvData.GenerateVideoMockData();

            mainPlayer.prepareCompleted += DisableBackgroundVideo;
            backgroundPlayer.prepareCompleted += DisableMainVideo;

            // start first clip
            PlayVideo(_allVideoElements[0]);
        }

        void OnDestroy() {
            mainPlayer.prepareCompleted -= DisableBackgroundVideo;
            backgroundPlayer.prepareCompleted -= DisableMainVideo;
        }

        private void DisableMainVideo(VideoPlayer source) {
            SetPlayerVisible(false);
        }

        private void DisableBackgroundVideo(VideoPlayer source) {
            SetPlayerVisible();
            view.StopBackgroundVideo();
            SetBackgroundInfo(_currentInfo.Background);
        }

        public void PlayVideo(VideoInfo info) {
            _currentInfo = info;
            view.PlayMainVideoClip(LoadVideo(_currentInfo.Name));

            // return all current navigation targets to pool
            ObjectPool.Instance.ReturnAllObjectsToPool();
        }

        public void PlayVideoByName(string videoName) {
            PlayVideo(_allVideoElements.FirstOrDefault(v => v.Name == videoName));
        }


        public VideoClip LoadVideo(string name) {
            return ResourceInfo.LoadVideoClipFromResources(name);
        }

        public Texture2D LoadStaticBackground(string name) {
            return ResourceInfo.LoadStaticBackgroundFromResources(name);
        }

        public VideoClip LoadDynamicBackground(string name) {
            return ResourceInfo.LoadDynamicBackgroundFromResources(name);
        }

        public void SetBackgroundPlayer() {
            GenerateNavigationElements(_currentInfo.Targets, view.transform);
            if (!_isBackgroundStatic) {
                backgroundPlayer.Play();
            } else {
                SetPlayerVisible(false);
            }
        }

        private void GenerateNavigationElements(TargetInfo[] targets, Transform viewTransform) {
            for (int i = 0; i < targets.Length; i++) {
                // rent new navigation targets from pool
                GameObject targetObject = ObjectPool.Instance.GetPooledObject();
                targetObject.SetActive(true);
                targetObject.transform.SetParent(viewTransform);
                targetObject.transform.localScale = Vector3.one;

                NavigationTargetClick navigationTarget = targetObject.GetComponent<NavigationTargetClick>();
                navigationTarget.SetTargetData(targets[i], this);
            }
        }

        private void SetBackgroundInfo(BackgroundInfo info) {
            if (info.Type == BackgroundType.Static) {
                _isBackgroundStatic = true;
                backgroundMaterial.mainTexture = LoadStaticBackground(info.Name);

                backgroundPlayer.transform.localEulerAngles = _staticLocalRotationEuler;
                backgroundPlayer.gameObject.GetComponent<Renderer>().material = backgroundMaterial;
                backgroundPlayer.clip = null;
            } else {
                _isBackgroundStatic = false;
                backgroundPlayer.transform.localEulerAngles = _dynamicLocalRotationEuler;
                backgroundPlayer.gameObject.GetComponent<Renderer>().material = videoMaterial;
                backgroundPlayer.clip = LoadDynamicBackground(info.Name);
            }
        }

        private void SetPlayerVisible(bool isMainVisible = true) {
            if (isMainVisible) {
                mainPlayer.transform.localPosition = new Vector3(0, 0, 0.01f);
                backgroundPlayer.transform.localPosition = new Vector3(0, 0, 0.1f);
            } else {
                mainPlayer.transform.localPosition = new Vector3(0, 0, 0.1f);
                backgroundPlayer.transform.localPosition = new Vector3(0, 0, 0.01f);
            }
            //mainPlayer.gameObject.SetActive(isMainVisible);
            //backgroundPlayer.gameObject.SetActive(!isMainVisible);
        }
    }
}