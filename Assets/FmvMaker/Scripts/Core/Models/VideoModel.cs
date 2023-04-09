using System;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class VideoModel : BaseUiElementModel {
        public string Name;
        public string VideoTarget;
        public bool IsLooping;
        public ClickableModel[] NavigationTargets;
        public ClickableModel[] ItemsToFind;
        public ClickableModel[] ItemsToUse;
        public bool AlreadyWatched;
    }
}