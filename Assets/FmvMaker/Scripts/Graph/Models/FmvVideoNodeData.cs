using System;
using UnityEngine;

namespace FmvMaker.Graph {
    [Serializable]
    public class FmvVideoNodeData {
        public GameObject Object { get; set; }
        public FmvGraphElementData Details { get; set; }

        public FmvVideoNodeData(GameObject dataGameObject, FmvGraphElementData detailDataObject) {
            Object = dataGameObject;
            Details = detailDataObject;
        }
    }
}