using System;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class FmvMakerConfig {
        public string AspectRatio;
        public string OnlineVideoURL;
        public string OnlineImageURL;
        public string LocalFilePath;
        public string SourceType;
    }
}