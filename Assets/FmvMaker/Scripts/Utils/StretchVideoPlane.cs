using UnityEngine;

namespace FmvMaker.Utils {
    public class StretchVideoPlane : MonoBehaviour {

        private float _ratio = 1.777777777f;

        void Start() {
            float height = 2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad);

            // opt. calculate camera FOV width
            //float width = height * Screen.width / Screen.height;
            //transform.localScale = new Vector3(width, height, 1);

            // apply scale for filling with optimal video height
            transform.localScale = new Vector3(height / _ratio, height, 1);
        }
    }
}