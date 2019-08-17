using System;

namespace FmvMaker.Models {
    [Serializable]
    public class FmvMakerConfig {
        public string AspectRatio;
        public string OnlineVideoURL;
        public string OnlineImageURL;
        public string LocalFilePath;
        public string SourceType;
    }
}