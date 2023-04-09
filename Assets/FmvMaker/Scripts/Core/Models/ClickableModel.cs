using System;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class ClickableModel : BaseUiElementModel {
        public string Name;
        public string Description;
        public string PickUpVideo;
        public string UsageVideo;
        public bool IsNavigation;
        public bool IsInInventory;
        public bool WasUsed;
    }
}