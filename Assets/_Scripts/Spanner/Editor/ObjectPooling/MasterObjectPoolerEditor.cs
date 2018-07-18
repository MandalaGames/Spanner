using UnityEngine;
using UnityEditor;

namespace Spanner {

    [CustomEditor(typeof(MasterObjectPooler))]
    public class MasterObjectPoolerEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            // Get the Master Pooler instance
            MasterObjectPooler master = (MasterObjectPooler) target;

            // When the button is clicked, pool all the objects in the list
            if (GUILayout.Button("Pool Objects")) {
                Debug.Log("Pooling Objects");
                master.PoolObjects();
            }
        }
    }
}
