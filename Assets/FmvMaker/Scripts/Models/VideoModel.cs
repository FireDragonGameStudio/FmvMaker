using System;
using System.Collections.Generic;

namespace FmvMaker.Models {
    [Serializable]
    public class VideoModel {
        public string Name;
        public bool IsLooping;
        public List<NavigationModel> NavigationTargets;
        public List<ItemModel> ItemsToFind;
        public List<ItemModel> ItemsToUse;
    }
}