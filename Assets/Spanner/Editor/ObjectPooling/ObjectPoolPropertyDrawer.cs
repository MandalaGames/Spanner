using UnityEditor;
using UnityEngine;

namespace Spanner {
    [CustomPropertyDrawer(typeof(ObjectPool))]
    public class ObjectPoolPropertyDrawer : PropertyDrawer {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Label contents
            GUIContent prefabLabelContent = new GUIContent("Prefab");
            GUIContent numberToPoolLabelContent = new GUIContent("Number");
            GUIContent startDisabledLabelContent = new GUIContent("Enabled");

            // Calculate rects
            var prefabLabelRect = new Rect(position.x, position.y, 50, position.height);
            var prefabRect = new Rect(position.x + 52, position.y, 90, position.height);
            var numberToPoolLabelRect = new Rect(position.x + 144, position.y, 50, position.height);
            var numberToPoolRect = new Rect(position.x + 196, position.y, 40, position.height);
            var startEnabledLabelRect = new Rect(position.x + 238, position.y, 50, position.height);
            var startEnabledRect = new Rect(position.x + 290, position.y, 15, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.LabelField(prefabLabelRect, prefabLabelContent);
            EditorGUI.PropertyField(prefabRect, property.FindPropertyRelative("prefabToPool"), GUIContent.none);
            EditorGUI.LabelField(numberToPoolLabelRect, numberToPoolLabelContent);
            EditorGUI.PropertyField(numberToPoolRect, property.FindPropertyRelative("numberToPool"), GUIContent.none);
            EditorGUI.LabelField(startEnabledLabelRect, startDisabledLabelContent);
            EditorGUI.PropertyField(startEnabledRect, property.FindPropertyRelative("startEnabled"), GUIContent.none);
            
            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
