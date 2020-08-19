﻿using System;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class VideoModel : BaseUiElementModel {
        public string Name;
        public bool IsLooping;
        public NavigationModel[] NavigationTargets;
        public ItemModel[] ItemsToFind;
        public ItemModel[] ItemsToUse;
        public bool AlreadyWatched;
    }
}