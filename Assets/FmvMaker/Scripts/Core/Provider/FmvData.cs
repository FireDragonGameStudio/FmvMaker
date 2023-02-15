using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using FmvMaker.Graph;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvData : MonoBehaviour {

        public Dictionary<string, FmvGraphElementData> gameData { get; set; } = new Dictionary<string, FmvGraphElementData>();
        public Dictionary<string, FmvGraphElementData> inventoryData { get; set; } = new Dictionary<string, FmvGraphElementData>();

        [Header("Key Bindings")]
        [SerializeField]
        private KeyCode ExportKey = KeyCode.X;

        [Header("Debug Settings")]
        [SerializeField]
        private bool loadDebugData;

        [Header("Data references")]
        [SerializeField]
        private TextAsset videoModelData = null;
        [SerializeField]
        private TextAsset clickableModelData = null;
        [SerializeField]
        private TextAsset onlineVideoSourceMappingData = null;

        private void Awake() {
            if (!videoModelData && !clickableModelData && !onlineVideoSourceMappingData) {
                Debug.LogWarning("No data available for FmvMaker. Check your FmvData references. FmvMaker will try to use DemoData.");
                videoModelData = Resources.Load<TextAsset>("DemoVideoData");
                clickableModelData = Resources.Load<TextAsset>("DemoClickableData");
            }
            if (loadDebugData) {
                LoadGameDataFromLocalFile();
            }
        }

        private void Update() {
            ExportVideoData();
        }

        private void ExportVideoData() {
            if (Input.GetKeyUp(ExportKey)) {
                //ExportVideoDataToLocalFile(allVideoElements, LoadFmvConfig.Config.LocalFilePath);
                ExportGameDatatoLocalFile();
            }
        }

        //private void ExportVideoDataToLocalFile(VideoModel[] videoElements, string localFilePath) {
        //    using (StreamWriter sw = new StreamWriter(Path.Combine(localFilePath, "FmvMakerDemoVideoData"))) {
        //        sw.Write(JsonUtility.ToJson(videoElements));
        //    }
        //}

        private void ExportGameDatatoLocalFile() {
            List<SaveGameModel> saveGameModelData = new List<SaveGameModel>();
            foreach (FmvGraphElementData elementData in gameData.Values) {
                saveGameModelData.Add(elementData.GetSaveGameModel());
            }
            //using (StreamWriter sw = new StreamWriter(Path.Combine(Application.persistentDataPath, "SaveGame.json"))) {
            using (StreamWriter sw = new StreamWriter(Path.Combine(Application.persistentDataPath, "SaveGameData.json"))) {
                SaveGameModelWrapper gameDataModelWrapper = new SaveGameModelWrapper();
                gameDataModelWrapper.GameDataList = saveGameModelData.ToArray();
                sw.Write(JsonUtility.ToJson(gameDataModelWrapper));
            }
        }

        private void LoadGameDataFromLocalFile() {
            string saveGameData = "";
            using (StreamReader sr = new StreamReader(Path.Combine(Application.persistentDataPath, "SaveGameData.json"))) {
                saveGameData = sr.ReadToEnd();
            }

            SaveGameModelWrapper gameDataModelWrapper = JsonUtility.FromJson<SaveGameModelWrapper>(saveGameData);
            gameData.Clear();
            foreach (SaveGameModel gameModelData in gameDataModelWrapper.GameDataList) {
                gameData.Add(gameModelData.Id, new FmvGraphElementData(gameModelData));
            }
        }

        public VideoModel[] GenerateVideoDataFromConfigurationFile() {
            return JsonUtility.FromJson<VideoModelWrapper>(videoModelData.text).VideoList;
        }

        public ClickableModel[] GenerateClickableDataFromConfigurationFile() {
            return JsonUtility.FromJson<ClickableModelWrapper>(clickableModelData.text).ClickableList;
        }

        public void GenerateOnlineVideoMappingData() {
            VideoOnlineSource[] videoMappingData = JsonUtility.FromJson<VideoOnlineSourceWrapper>(onlineVideoSourceMappingData.text).OnlineVideoSourceMappingList;
            ResourceVideoInfo.SetOnlineVideoMappungData(videoMappingData);
        }
    }
}