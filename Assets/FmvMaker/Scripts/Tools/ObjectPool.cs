using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Tools {
    public class ObjectPool : MonoBehaviour {

        public static ObjectPool Instance;

        [SerializeField]
        private List<GameObject> pooledTargetObjects;
        [SerializeField]
        private List<GameObject> pooledVideoObjects;
        [SerializeField]
        private GameObject targetObjectToPool;
        [SerializeField]
        private GameObject videoObjectToPool;

        void Awake() {
            Instance = this;
        }

        void Start() {
            pooledTargetObjects = new List<GameObject>();
        }

        public GameObject GetPooledTargetObject() {
            for (int i = 0; i < pooledTargetObjects.Count; i++) {
                if (!pooledTargetObjects[i].activeInHierarchy) {
                    return pooledTargetObjects[i];
                }
            }

            // no pooled objects available
            GameObject newObj = Instantiate(targetObjectToPool);
            newObj.SetActive(false);
            pooledTargetObjects.Add(newObj);
            return newObj;
        }

        public GameObject GetPooledVideoObject() {
            for (int i = 0; i < pooledVideoObjects.Count; i++) {
                if (!pooledVideoObjects[i].activeInHierarchy) {
                    return pooledVideoObjects[i];
                }
            }

            // no pooled objects available
            GameObject newObj = Instantiate(videoObjectToPool);
            pooledVideoObjects.Add(newObj);
            return newObj;
        }


        public void ReturnAllTargetObjectsToPool() {
            for (int i = 0; i < pooledTargetObjects.Count; i++) {
                pooledTargetObjects[i].SetActive(false);
            }
        }
    }
}