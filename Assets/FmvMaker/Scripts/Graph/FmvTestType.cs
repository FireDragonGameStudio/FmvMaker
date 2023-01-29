using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    [Inspectable]
    public class FmvTestType : MonoBehaviour {
        [Inspectable]
        private FmvFacadeType firstPlayer = null;
        [Inspectable]
        private FmvFacadeType secondPlayer = null;
    }
}