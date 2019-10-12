using FmvMaker.Models;
using System.IO;
using UnityEngine;

namespace FmvMaker.Utils {
    public static class LoadFmvConfig {

        private static FmvMakerConfig _config;

        public static FmvMakerConfig Config {
            get {
                if (_config == null) {
                    _config = JsonUtility.FromJson<FmvMakerConfig>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "FmvMaker", "FmvMakerConfig.json")));
                }
                return _config;
            }
        }
    }
}