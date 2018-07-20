using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ObjectPool {
    public GameObject prefabToPool;
    public int numberToPool;
    public bool startEnabled;
    public string poolID;

    private List<GameObject> _instances;
    private Transform _parentTransform;

    public void Clear(Transform master) {
        _instances = new List<GameObject>();
        if (_parentTransform != null) {
            UnityEngine.Object.Destroy(_parentTransform.gameObject);
        }

        _parentTransform = (new GameObject(poolID.Equals("") ? prefabToPool.name : poolID)).transform;
        _parentTransform.parent = master;
    }

    public void Initialize() {
        for (int i = 0; i < numberToPool; i++) {
            GameObject instance = UnityEngine.Object.Instantiate(prefabToPool, _parentTransform);
            instance.SetActive(startEnabled);
            _instances.Add(instance);
        }
    }
}
