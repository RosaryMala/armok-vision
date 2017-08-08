using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(CreatureSpriteCollection))]
public class CreatureSpriteCollectionEditor : Editor
{
    private ReorderableList list;
    Material mat;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("spriteLayers"), true, true, true, true);

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("spriteSource"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + 60, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("colorSource"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + 60 + 60, rect.y, rect.width - 60 - 60 - 60 - 60 - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("token"), GUIContent.none);
            EditorGUI.PropertyField(
                    new Rect(rect.x + rect.width - 60 - 60 - EditorGUIUtility.singleLineHeight, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("spriteTexture"), GUIContent.none);
            EditorGUI.PropertyField(
                    new Rect(rect.x + rect.width - 60 - EditorGUIUtility.singleLineHeight, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("color"), GUIContent.none);
            EditorGUI.PropertyField(
                   new Rect(rect.x + rect.width - EditorGUIUtility.singleLineHeight, rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight),
                   element.FindPropertyRelative("preview"), GUIContent.none);
        };
        mat = new Material(Shader.Find("Hidden/TransparentPreview"));
    }

    public override void OnInspectorGUI()
    {
        var rect = EditorGUILayout.GetControlRect(false, 256);
        foreach (var layer in ((CreatureSpriteCollection)target).spriteLayers)
        {
            if (layer.preview)
            {
                if (layer.colorSource == CreatureSpriteLayer.ColorSource.None)
                    mat.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                else
                    mat.color = layer.color;
                EditorGUI.DrawPreviewTexture(rect, layer.spriteTexture, mat, ScaleMode.ScaleToFit);
            }
        }
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
