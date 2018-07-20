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
            foreach (ObjectPool pool in pools) {
                string enabled = pool.startEnabled ? "enabled" : "disabled";
                Debug.Log("Pooling " + pool.numberToPool + " " + enabled + " instances of " + pool.prefabToPool.name);
            }
        }
    }
}
