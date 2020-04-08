using UnityEngine;

namespace FmvMaker.Core.Interfaces {
    public interface IObjectPool {
        GameObject GetPooledObject(GameObject prefab);
        void RemoveObjectFromPool(GameObject poolObject);
        void ReturnAllObjectsToPool();
    }
}