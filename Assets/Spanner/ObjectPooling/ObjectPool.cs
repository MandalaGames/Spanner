using UnityEngine;
using System;
using System.Collections.Generic;

namespace Spanner {
    [Serializable]
    public class ObjectPool {
        public GameObject prefabToPool;
        public int numberToPool;
        public bool startEnabled;
        public string poolID;

        [SerializeField, HideInInspector]
        private List<GameObject> _instances;
        [SerializeField, HideInInspector]
        private Transform _parentTransform;

        public void Clear(Transform master) {
            _instances = new List<GameObject>();
            if (_parentTransform != null) {
                UnityEngine.Object.DestroyImmediate(_parentTransform.gameObject);
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

        public GameObject Get() {
            GameObject instance = _instances.Find(go => !go.activeInHierarchy);
            if (instance != null) {
                instance.SetActive(true);
            }
            return instance;
        }

        public void Test() {
            Debug.Log(_instances.ToArray().Length);
        }
    }
}
