using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Tools {
    public class ObjectPool : MonoBehaviour {

        public static ObjectPool Instance;

        public List<GameObject> pooledObjects;
        public GameObject objectToPool;
        public int amountToPool;

        void Awake() {
            Instance = this;
        }

        void Start() {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++) {
                
            }
        }

        public GameObject GetPooledObject() {
            for (int i = 0; i < pooledObjects.Count; i++) {
                if (!pooledObjects[i].activeInHierarchy) {
                    return pooledObjects[i];
                }
            }

            // no pooled objects available
            GameObject newObj = Instantiate(objectToPool);
            newObj.SetActive(false);
            pooledObjects.Add(newObj);
            return newObj;
        }

        public void ReturnAllObjectsToPool() {
            for (int i = 0; i < pooledObjects.Count; i++) {
                pooledObjects[i].SetActive(false);
            }
        }
    }
}