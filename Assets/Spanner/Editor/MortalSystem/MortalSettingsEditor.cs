using UnityEngine;
using UnityEditor;

namespace Spanner {
    [CustomEditor(typeof(MortalSettings))]
    public class MortalSettingsEditor : Editor {

        private struct SerializedProperties {
            public SerializedProperty randomizeHealth;
            public SerializedProperty maxHealthCap;
            public SerializedProperty minHealthCap;
            public SerializedProperty maxStartingHealth;
            public SerializedProperty minStartingHealth;
            public SerializedProperty maxHealth;
            public SerializedProperty startingHealth;
            public SerializedProperty initializeOnStart;
        }

        private SerializedProperties _properties;

        private void OnEnable() {
            _properties.randomizeHealth = serializedObject.FindProperty("randomizeHealth");
            _properties.maxHealthCap = serializedObject.FindProperty("maxHealthCap");
            _properties.minHealthCap = serializedObject.FindProperty("minHealthCap");
            _properties.maxStartingHealth = serializedObject.FindProperty("maxStartingHealth");
            _properties.minStartingHealth = serializedObject.FindProperty("minStartingHealth");
            _properties.maxHealth = serializedObject.FindProperty("maxHealth");
            _properties.startingHealth = serializedObject.FindProperty("startingHealth");
            _properties.initializeOnStart = serializedObject.FindProperty("initializeOnStart");
        }

        public override void OnInspectorGUI() {
            // Get new property values from the serializedObject
            serializedObject.Update();

            // Always draw the checkbox for 'Randomize Health'
            EditorGUILayout.PropertyField(_properties.randomizeHealth);

            // Change the inspector layout depending on the value of 'Randomize Health'
            if (_properties.randomizeHealth.boolValue) {
                EditorGUILayout.PropertyField(_properties.minHealthCap);
                EditorGUILayout.PropertyField(_properties.maxHealthCap);
                EditorGUILayout.PropertyField(_properties.minStartingHealth);
                EditorGUILayout.PropertyField(_properties.maxStartingHealth);
            } else {
                EditorGUILayout.PropertyField(_properties.maxHealth);
                EditorGUILayout.PropertyField(_properties.startingHealth);
            }

            // Always draw the checkbox for 'Initialize On Startup'
            EditorGUILayout.PropertyField(_properties.initializeOnStart);

            SanitizeInputs();

            // Apply changes made from the inspector to the object
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Forces the values input by the inspector to be within an appropriate range
        /// </summary>
        private void SanitizeInputs() {
            if (_properties.randomizeHealth.boolValue) {
                _properties.minHealthCap.intValue = Mathf.Clamp(_properties.minHealthCap.intValue, 1, int.MaxValue);
                _properties.maxHealthCap.intValue = Mathf.Clamp(_properties.maxHealthCap.intValue, _properties.minHealthCap.intValue, int.MaxValue);
                _properties.minStartingHealth.intValue = Mathf.Clamp(_properties.minStartingHealth.intValue, 1, _properties.maxHealthCap.intValue);
                _properties.maxStartingHealth.intValue = Mathf.Clamp(_properties.maxStartingHealth.intValue, _properties.minStartingHealth.intValue, _properties.maxHealthCap.intValue);
            } else {
                _properties.maxHealth.intValue = Mathf.Clamp(_properties.maxHealth.intValue, 1, int.MaxValue);
                _properties.startingHealth.intValue = Mathf.Clamp(_properties.startingHealth.intValue, 1, _properties.maxHealth.intValue);
            }
        }
    }
}
