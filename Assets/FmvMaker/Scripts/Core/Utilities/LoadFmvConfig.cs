using FmvMaker.Core.Models;
using System.IO;
using UnityEngine;

namespace FmvMaker.Core.Utilities {
    public static class LoadFmvConfig {

        private static FmvMakerConfig _config;

        public static FmvMakerConfig Config {
            get {
                if (_config == null) {
                    string configText = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "FmvMaker", "FmvMakerConfig.json"));
                    _config = JsonUtility.FromJson<FmvMakerConfig>(configText);
                }
                return _config;
            }
        }
    }
}