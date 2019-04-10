using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Utils {
    public class ObjectPool : MonoBehaviour {

        public static ObjectPool Instance;

        //[SerializeField]
        //[SerializeField]
        //private List<GameObject> pooledVideoObjects;
        [SerializeField]
        private GameObject targetObjectPrefab;
        //[SerializeField]
        //private GameObject videoObjectToPool;

        private List<GameObject> pooledTargetObjects = new List<GameObject>();

        void Awake() {
            Instance = this;
        }

        public GameObject GetPooledTargetObject() {
            for (int i = 0; i < pooledTargetObjects.Count; i++) {
                if (!pooledTargetObjects[i].activeInHierarchy) {
                    return pooledTargetObjects[i];
                }
            }

            // no pooled objects available
            GameObject newObj = Instantiate(targetObjectPrefab);
            newObj.SetActive(false);
            pooledTargetObjects.Add(newObj);
            return newObj;
        }

        //public GameObject GetPooledVideoObject() {
        //    for (int i = 0; i < pooledVideoObjects.Count; i++) {
        //        if (!pooledVideoObjects[i].activeInHierarchy) {
        //            return pooledVideoObjects[i];
        //        }
        //    }

        //    // no pooled objects available
        //    GameObject newObj = Instantiate(videoObjectToPool);
        //    pooledVideoObjects.Add(newObj);
        //    return newObj;
        //}


        public void ReturnAllTargetObjectsToPool() {
            for (int i = 0; i < pooledTargetObjects.Count; i++) {
                pooledTargetObjects[i].SetActive(false);
            }
        }
    }
}