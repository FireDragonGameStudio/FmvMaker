﻿using FmvMaker.Core.Models;
using System.IO;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvData : MonoBehaviour {

        //[Header("Key Bindings")]
        //[SerializeField]
        //private KeyCode ExportKey = KeyCode.X;

        [Header("Data references")]
        [SerializeField]
        private TextAsset VideoModelData;
        [SerializeField]
        private TextAsset ClickableModelData;

        private void Awake() {
            if (!VideoModelData || !ClickableModelData) {
                VideoModelData = Resources.Load<TextAsset>("FmvMakerDemoVideoData");
                ClickableModelData = Resources.Load<TextAsset>("FmvMakerDemoClickableData");
            }
        }

        //private void Update() {
        //    ExportVideoData();
        //}

        //private void ExportVideoData() {
        //    if (Input.GetKeyUp(ExportKey)) {
        //        ExportVideoDataToLocalFile(allVideoElements, LoadFmvConfig.Config.LocalFilePath);
        //    }
        //}

        private void ExportVideoDataToLocalFile(VideoModel[] videoElements, string localFilePath) {
            using (StreamWriter sw = new StreamWriter(Path.Combine(localFilePath, "FmvMakerDemoVideoData"))) {
                sw.Write(JsonUtility.ToJson(videoElements));
            }
        }

        public VideoModel[] GenerateVideoDataFromLocalFile() {
            return JsonUtility.FromJson<VideoModelWrapper>(VideoModelData.text).VideoList;
        }

        public ClickableModel[] GenerateItemDataFromLocalFile() {
            return JsonUtility.FromJson<ClickableModelWrapper>(ClickableModelData.text).ItemList;
        }
    }
}