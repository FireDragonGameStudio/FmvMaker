using UnityEngine;

namespace FmvMaker.Core.Utilities {
    public class ContinuousObjectRotation : MonoBehaviour {

        [SerializeField]
        private float zAngle = -0.5f;

        private void Update() {
            transform.Rotate(0, 0, zAngle, Space.Self);
        }
    }
}
