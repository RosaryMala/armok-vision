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

            // Draw label
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), property.isExpanded, label);
            EditorGUI.PropertyField(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("tag"), GUIContent.none);

            if (property.isExpanded)
            {
                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                Rect runningPosition = new Rect(position.x, position.y, position.width - (EditorGUIUtility.singleLineHeight * 5), EditorGUIUtility.singleLineHeight);
                runningPosition.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(runningPosition, property.FindPropertyRelative("color"));
                runningPosition.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(runningPosition, property.FindPropertyRelative("texture"));
                runningPosition.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(runningPosition, property.FindPropertyRelative("normal"));
                runningPosition.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(runningPosition, property.FindPropertyRelative("heightMap"));
                runningPosition.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(runningPosition, property.FindPropertyRelative("aoMap"));

                //draw the preview texture.
                if (mat == null)
                {
                    mat = new Material(Shader.Find("Hidden/ColorOverlay"));
                }
                mat.color = property.FindPropertyRelative("color").colorValue;

                var texture = (Texture2D)property.FindPropertyRelative("texture").objectReferenceValue;

                var previewRect = new Rect(
                            position.x + position.width - (EditorGUIUtility.singleLineHeight * 5),
                        position.y + EditorGUIUtility.singleLineHeight,
                        EditorGUIUtility.singleLineHeight * 5,
                        EditorGUIUtility.singleLineHeight * 5);

                if (texture == null)
                    EditorGUI.DrawRect(previewRect, new Color(mat.color.r, mat.color.g, mat.color.b));
                else
                    EditorGUI.DrawPreviewTexture(previewRect, texture, mat);

                EditorGUI.indentLevel = indent;

            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 6 : EditorGUIUtility.singleLineHeight;
        }
    }
}
