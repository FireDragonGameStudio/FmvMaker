using System;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class FmvMakerConfig {
        public string AspectRatio;
        public string VideoSourceType;
        public string OnlineVideoURL;
        public string ImageSourceType;
        public string OnlineImageURL;
        public string LocalFilePath;
    }
}