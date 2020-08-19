using System;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class NavigationModel : BaseUiElementModel {
        public string Name;
        public string NextVideo = "";
    }
}