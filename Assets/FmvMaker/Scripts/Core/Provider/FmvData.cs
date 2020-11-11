using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using System.IO;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvData : MonoBehaviour {

        //[Header("Key Bindings")]
        //[SerializeField]
        //private KeyCode ExportKey = KeyCode.X;

        [Header("Data references")]
        [SerializeField]
        private TextAsset VideoModelData = null;
        [SerializeField]
        private TextAsset ClickableModelData = null;
        [SerializeField]
        private TextAsset OnlineVideoSourceMappingData = null;

        private void Awake() {
            if (!VideoModelData || !ClickableModelData) {
                Debug.LogWarning("No data available for FmvMaker. Check your FmvData references. FmvMaker will try to use DemoData.");
                VideoModelData = Resources.Load<TextAsset>("DemoVideoData");
                ClickableModelData = Resources.Load<TextAsset>("DemoClickableData");
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

        //private void ExportVideoDataToLocalFile(VideoModel[] videoElements, string localFilePath) {
        //    using (StreamWriter sw = new StreamWriter(Path.Combine(localFilePath, "FmvMakerDemoVideoData"))) {
        //        sw.Write(JsonUtility.ToJson(videoElements));
        //    }
        //}

        public VideoModel[] GenerateVideoDataFromConfigurationFile() {
            return JsonUtility.FromJson<VideoModelWrapper>(VideoModelData.text).VideoList;
        }

        public ClickableModel[] GenerateClickableDataFromConfigurationFile() {
            return JsonUtility.FromJson<ClickableModelWrapper>(ClickableModelData.text).ClickableList;
        }

        public void GenerateOnlineVideoMappingData() {
            VideoOnlineSource[] videoMappingData = JsonUtility.FromJson<VideoOnlineSourceWrapper>(OnlineVideoSourceMappingData.text).OnlineVideoSourceMappingList;
            ResourceVideoInfo.SetOnlineVideoMappungData(videoMappingData);
        }
    }
}