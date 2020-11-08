using FmvMaker.Core.Models;
using System.IO;
using UnityEngine;

namespace FmvMaker.Core.Utilities {
    public static class LoadFmvConfig {

        private static FmvMakerConfig config;

        public static FmvMakerConfig Config {
            get {
                if (config == null) {
                    TextAsset configText = Resources.Load<TextAsset>("FmvMakerConfig");
                    LoadFmvConfig.config = JsonUtility.FromJson<FmvMakerConfig>(configText.text);
                }
                return config;
            }
        }
    }
}