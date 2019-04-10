using FmvMaker.Models;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FmvMaker.Utils {
    public static class LoadFmvConfig {

        private static FmvMakerConfig _config;

        public static FmvMakerConfig LoadConfig() {
            if (_config == null) {
                _config = JsonConvert.DeserializeObject<FmvMakerConfig>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "FmvMaker", "FmvMaker.json")));
            }
            return _config;
        }
    }
}