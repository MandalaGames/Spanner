using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Spanner {
    public class MasterObjectPooler : MonoBehaviour {
        [SerializeField]
        public List<ObjectPool> pools;

        private static MasterObjectPooler _instance;
        public static MasterObjectPooler Instance {
            get {
                if (_instance == null) {
                    Debug.LogWarning("You didn't manually set up your Object Pooler yet. Creating blank Object Pooler");
                    GameObject go = new GameObject("Master Object Pooler");
                    _instance = go.AddComponent<MasterObjectPooler>();
                }
                return _instance;
            }
        }

        private void OnEnable() {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }

        /// <summary>
        /// Should only be called by the button in the inspector. Loops through each pool and instantiates all the
        /// objects for that pool
        /// </summary>
        public void PoolObjects() {

            ClearAllPools();

            foreach (ObjectPool pool in pools) {
                if (pool.prefabToPool != null) {
                    string enabled = pool.startEnabled ? "enabled" : "disabled";
                    Debug.Log("Pooling " + pool.numberToPool + " " + enabled + " instances of " + pool.prefabToPool.name);
                    CreatePool(pool);
                } else {
                    Debug.LogWarning("One of your object pools didn't have a prefab specified. Skipping pool #" + pools.IndexOf(pool));
                }
            }
        }

        private void ClearAllPools() {
            foreach (Transform child in transform) {
                DestroyImmediate(child.gameObject);
            }
        }

        private void CreatePool(ObjectPool pool) {
            pool.Clear(transform);
            pool.Initialize();
        }

        public GameObject Get(string id) {
            ObjectPool pool = pools.Find(p => (p.poolID != null && p.poolID.Equals(id)));
            if (pool != null) {
                return pool.Get();
            } else {
                Debug.LogWarning("No Object Pool with ID " + id + "... Did you remember to click \"Pool Objects\"?");
                return null;
            }
        }

        public GameObject Get(GameObject prefab) {
            ObjectPool pool = pools.Find(p => prefab == p.prefabToPool);
            if (pool != null) {
                return pool.Get();
            } else {
                Debug.LogWarning("No Object Pool found containing instances of prefab " + prefab.name 
                    + "... Did you remember to click \"Pool Objects\"?");
                return null;
            }
        }

        public void Return(GameObject gameObject) {
            gameObject.SetActive(false);
        }
    }
}
