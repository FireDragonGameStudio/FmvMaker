using FmvMaker.Models;
using FmvMaker.Tools;
using FmvMaker.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

// Idea: Crowd clients on LAN can vote for a way to go -> Mirror

namespace FmvMaker.Presenter {
    public class VideoPresenter : MonoBehaviour {

        [SerializeField]
        private VideoView _view;
        [SerializeField]
        private Material videoMaterial;
        [SerializeField]
        private Material backgroundMaterial;

        private List<VideoInfo> _allVideoElements;
        private bool _isBackgroundStatic;
        private VideoInfo _currentInfo;

        private Vector3 _staticLocalRotationEuler = new Vector3(90, 180, 0);
        private Vector3 _dynamicLocalRotationEuler = new Vector3(180, -90, 90);

        private FmvMakerConfig _config => LoadFmvConfig.LoadConfig();

        void Awake() {
            if (!_view) {
                Debug.LogError("No view for presenter set. I'll try to find it automatically. Please check if view is set, before starting playmode again.", this);
                _view = FindObjectOfType<VideoView>();
            }
        }

        void Start() {

            FmvMakerConfig config = LoadFmvConfig.LoadConfig();
            Debug.Log(config.SourceType);

            _allVideoElements = LoadFmvData.GenerateVideoMockData();

            // start first clip
            PlayVideo(_allVideoElements[0]);
        }

        public void DisableBackgroundVideo() {
            _view.SetPlayerVisible();
            _view.StopBackgroundVideo();
            SetBackgroundInfo(_currentInfo.Background);
        }

        public void PlayVideo(VideoInfo info) {
            _currentInfo = info;

            // either load from file system/online or resources
            switch (_config.SourceType) {
                case VideoSource.Online:
                case VideoSource.Offline:
                    _view.PlayMainVideoClip(_currentInfo.Name);
                    break;
                default:
                    _view.PlayMainVideoClip(LoadVideo(_currentInfo.Name));
                    break;
            }

            // return all current navigation targets to pool
            ObjectPool.Instance.ReturnAllTargetObjectsToPool();
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
            GenerateNavigationElements(_currentInfo.Targets, _view.transform);
            if (!_isBackgroundStatic) {
                _view.StartBackgroundVideo();
            } else {
                _view.SetPlayerVisible(false);
            }
        }

        private void GenerateNavigationElements(TargetInfo[] targets, Transform viewTransform) {
            for (int i = 0; i < targets.Length; i++) {
                // rent new navigation targets from pool
                GameObject targetObject = ObjectPool.Instance.GetPooledTargetObject();
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
                _view.SetStaticBackgroundInfo(_staticLocalRotationEuler, backgroundMaterial);
            } else {
                _isBackgroundStatic = false;
                _view.SetDynamicBackgroundInfo(_dynamicLocalRotationEuler, videoMaterial, LoadDynamicBackground(info.Name));
            }
        }
    }
}