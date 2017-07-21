using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MaterialStore
{
    [CustomPropertyDrawer(typeof(MaterialTextureSet))]
    public class MaterialTextureSetDrawer : PropertyDrawer
    {
        Material mat;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(
                new Rect(
                    position.x,
                    position.y,
                    EditorGUIUtility.singleLineHeight * 4,
                    EditorGUIUtility.singleLineHeight
                    ), property.FindPropertyRelative("color"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(
                    position.x + EditorGUIUtility.singleLineHeight * 4,
                    position.y,
                    position.width - EditorGUIUtility.singleLineHeight * 10,
                    EditorGUIUtility.singleLineHeight
                ), property.FindPropertyRelative("tag"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(
                    position.width - EditorGUIUtility.singleLineHeight * 6,
                    position.y,
                    EditorGUIUtility.singleLineHeight * 3,
                    EditorGUIUtility.singleLineHeight
                ), property.FindPropertyRelative("patternIndex"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(
                    position.width - EditorGUIUtility.singleLineHeight * 3,
                    position.y,
                    EditorGUIUtility.singleLineHeight * 3,
                    EditorGUIUtility.singleLineHeight
                ), property.FindPropertyRelative("shapeIndex"), GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
