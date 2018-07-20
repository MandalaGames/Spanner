using UnityEngine;
using System.Collections.Generic;

namespace Spanner {
    public class MasterObjectPooler : MonoBehaviour {
        [SerializeField]
        public List<ObjectPool> pools;

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
    }
}
