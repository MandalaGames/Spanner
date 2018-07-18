using UnityEngine;
using UnityEditor;

namespace Spanner {
    [CustomEditor(typeof(MortalSettings))]
    public class MortalSettingsEditor : Editor {
        public override void OnInspectorGUI() {
            // Get the Mortal Settings instance
            MortalSettings settings = (MortalSettings) target;

            // Always draw the checkbox for 'Randomize Health'
            SerializedProperty randomizeHealth = serializedObject.FindProperty("randomizeHealth");
            EditorGUILayout.PropertyField(randomizeHealth);

            // Change the inspector layout depending on the value of 'Randomize Health'
            if (settings.randomizeHealth) {
                SerializedProperty maxHealthCap = serializedObject.FindProperty("maxHealthCap");
                SerializedProperty minHealthCap = serializedObject.FindProperty("minHealthCap");
                SerializedProperty maxStartingHealth = serializedObject.FindProperty("maxStartingHealth");
                SerializedProperty minStartingHealth = serializedObject.FindProperty("minStartingHealth");

                EditorGUILayout.PropertyField(minHealthCap);
                EditorGUILayout.PropertyField(maxHealthCap);
                EditorGUILayout.PropertyField(minStartingHealth);
                EditorGUILayout.PropertyField(maxStartingHealth);
            } else {
                SerializedProperty maxHealth = serializedObject.FindProperty("maxHealth");
                SerializedProperty startingHealth = serializedObject.FindProperty("startingHealth");

                EditorGUILayout.PropertyField(maxHealth);
                EditorGUILayout.PropertyField(startingHealth);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
