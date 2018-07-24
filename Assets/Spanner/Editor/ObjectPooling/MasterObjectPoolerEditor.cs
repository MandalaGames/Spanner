using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Spanner {
    [CustomEditor(typeof(MasterObjectPooler))]
    public class MasterObjectPoolerEditor : Editor {
        public override void OnInspectorGUI() {
            // Get new property values from the serializedObject
            serializedObject.Update();

            // Get the Master Pooler instance
            MasterObjectPooler master = (MasterObjectPooler) target;

            // Header for Object Pool list
            GUILayout.Label("Object Pools", EditorStyles.boldLabel);

            // Button for adding new pools
            if (GUILayout.Button("Add New Object Pool")) {
                master.pools.Add(new ObjectPool());
            }

            // Button for resetting all pools
            if (GUILayout.Button("Reset")) {
                master.pools.RemoveAll(p => true);
            }

            // Draw a property field for each of the object pools
            SerializedProperty poolsProperty = serializedObject.FindProperty("pools");
            if (poolsProperty.isArray) {
                for (int i = 0; i < poolsProperty.arraySize; i++) {
                    GUILayout.BeginHorizontal();
                    // Display the object pool
                    EditorGUILayout.PropertyField(poolsProperty.GetArrayElementAtIndex(i));
                    
                    // Button for removing the object pool
                    if (GUILayout.Button("x", GUILayout.Width(20))) {
                        master.pools.RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }
            }

            // When the button is clicked, pool all the objects in the list
            if (GUILayout.Button("Pool Objects")) {
                Debug.Log("Pooling Objects");
                master.PoolObjects();
            }

            // Apply changes made from the inspector to the object
            serializedObject.ApplyModifiedProperties();
        }
    }
}
