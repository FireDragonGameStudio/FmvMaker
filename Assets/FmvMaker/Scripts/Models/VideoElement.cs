using System;

namespace FmvMaker.Models {
    [Serializable]
    public class VideoElement {
        public string Name;
        public bool IsLooping;
        public NavigationModel[] NavigationTargets;
        public string[] Items;
    }
}