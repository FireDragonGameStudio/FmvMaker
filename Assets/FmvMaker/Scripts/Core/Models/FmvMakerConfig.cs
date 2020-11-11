using System;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class FmvMakerConfig {
        public string AspectRatio;
        public string VideoSourceType;
        public string ImageSourceType;
        public string LocalFilePath;
    }
}