using FmvMaker.Core.Models;
using System.IO;
using UnityEngine;

namespace FmvMaker.Core.Utilities {
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