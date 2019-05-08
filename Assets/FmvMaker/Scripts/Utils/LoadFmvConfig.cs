using FmvMaker.Models;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace FmvMaker.Utils {
    public static class LoadFmvConfig {

        private static FmvMakerConfig _config;

        public static FmvMakerConfig Config {
            get {
                if (_config == null) {
                    _config = JsonConvert.DeserializeObject<FmvMakerConfig>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "FmvMaker", "FmvMakerConfig.json")));
                }
                return _config;
            }
        }
    }
}