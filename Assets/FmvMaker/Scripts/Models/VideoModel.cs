using System;

namespace FmvMaker.Models {
    [Serializable]
    public class VideoModel {
        public string Name;
        public bool IsLooping;
        public NavigationModel[] NavigationTargets;
        public ItemModel[] ItemsToFind;
        public ItemModel[] ItemsToUse;
    }
}