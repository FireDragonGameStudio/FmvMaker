using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Utils {
    public class ObjectPool : MonoBehaviour {

        public static ObjectPool Instance;

        [SerializeField]
        private GameObject _navigationTargetObjectPrefab = null;
        [SerializeField]
        private GameObject _itemObjectPrefab = null;

        private List<GameObject> _pooledNavigationTargetObjects = new List<GameObject>();
        private List<GameObject> _pooledItemObjects = new List<GameObject>();

        private void Awake() {
            Instance = this;
        }

        public GameObject GetPooledTargetObject() {
            return GetPooledObject(ref _pooledNavigationTargetObjects, _navigationTargetObjectPrefab);
        }

        public GameObject GetPooledItemObject() {
            return GetPooledObject(ref _pooledItemObjects, _itemObjectPrefab);
        }

        private GameObject GetPooledObject(ref List<GameObject> gameObjects, GameObject prefab) {
            for (int i = 0; i < gameObjects.Count; i++) {
                if (!gameObjects[i].activeInHierarchy) {
                    return gameObjects[i];
                }
            }

            // no pooled objects available
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(false);
            gameObjects.Add(newObj);
            return newObj;
        }

        public void ReturnAllTargetObjectsToPool() {
            ReturnAllObjectsToPool(ref _pooledNavigationTargetObjects);
        }

        public void ReturnAllItemObjectsToPool() {
            ReturnAllObjectsToPool(ref _pooledItemObjects);
        }

        private void ReturnAllObjectsToPool(ref List<GameObject> gameObjects) {
            for (int i = 0; i < gameObjects.Count; i++) {
                gameObjects[i].SetActive(false);
            }
        }
    }
}