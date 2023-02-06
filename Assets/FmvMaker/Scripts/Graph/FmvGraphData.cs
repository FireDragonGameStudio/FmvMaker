using FmvMaker.Core.Provider;
using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    [Inspectable]
    public class FmvGraphData {

        [Inspectable]
        private GameObject fmvVideoViewObject;

        private FmvVideoView fmvVideoView;

        public FmvGraphData() {
            fmvVideoView = fmvVideoViewObject.GetComponent<FmvVideoView>();
            Debug.Log("FmvVideoView: ", fmvVideoView);
        }
    }
}