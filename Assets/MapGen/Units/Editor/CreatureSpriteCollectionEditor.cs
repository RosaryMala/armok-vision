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
    private int rectID;
    private Vector2 listPosition;
    private SerializedProperty raceProp;
    private SerializedProperty casteProp;
    private SerializedProperty professionProp;
    private SerializedProperty specialProp;
    private SerializedProperty sizeProp;
    private int maxLayers;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("spriteLayers"), true, true, true, true);
        raceProp = serializedObject.FindProperty("race");
        casteProp = serializedObject.FindProperty("caste");
        professionProp = serializedObject.FindProperty("profession");
        specialProp = serializedObject.FindProperty("special");
        sizeProp = serializedObject.FindProperty("standardSize");

        mat = new Material(Shader.Find("Hidden/TransparentPreview"));
        rectID = Shader.PropertyToID("_Rect");

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            //left side
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 40, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("spriteSource"), GUIContent.none);
            rect.xMin += 40;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 40, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("colorSource"), GUIContent.none);
            rect.xMin += 40;
            if(element.FindPropertyRelative("spriteSource").enumValueIndex == (int)CreatureSpriteLayer.SpriteSource.Bodypart)
            {
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, 40, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("hairType"), GUIContent.none);
                rect.xMin += 40;
                if (element.FindPropertyRelative("hairType").enumValueIndex != (int)CreatureSpriteLayer.HairType.None)
                {
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, 40, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("hairStyle"), GUIContent.none);
                    rect.xMin += 40;
                    EditorGUI.PropertyField(
                      new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight),
                      element.FindPropertyRelative("hairMin"), GUIContent.none);
                    rect.xMin += 30;
                    EditorGUI.PropertyField(
                      new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight),
                      element.FindPropertyRelative("hairMax"), GUIContent.none);
                    rect.xMin += 30;
                }
            }
            //right side
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - EditorGUIUtility.singleLineHeight, rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("preview"), GUIContent.none);
            rect.xMax -= EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 35, rect.y, 35, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("color"), GUIContent.none);
            rect.xMax -= 35;
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 60, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("spriteTexture"), GUIContent.none);
            rect.xMax -= 60;
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 80, rect.y, 80, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("positionOffset"), GUIContent.none);
            rect.xMax -= 80;
            //center
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("token"), GUIContent.none);
        };

        list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Sprite Layers"); };

        list.onAddCallback = (ReorderableList list) =>
        {
            int index = list.index;
            if (index < 0)
                index = list.count;
            list.serializedProperty.InsertArrayElementAtIndex(index);
            EditorUtility.SetDirty(target);
        };

        maxLayers = list.serializedProperty.arraySize;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var collection = (CreatureSpriteCollection)target;
        EditorGUILayout.PropertyField(raceProp);
        EditorGUILayout.PropertyField(casteProp);
        EditorGUILayout.PropertyField(professionProp);
        EditorGUILayout.PropertyField(specialProp);
        EditorGUILayout.PropertyField(sizeProp);
        if (collection.spriteLayers != null && collection.spriteLayers.Count > 0)
        {
            var rect = EditorGUILayout.GetControlRect(false, 256);
            rect.x += ((rect.width - rect.height) / 2.0f);
            rect.width = rect.height;
            int count = 0;
            foreach (var layer in ((CreatureSpriteCollection)target).spriteLayers)
            {
                if (count > maxLayers)
                    break;
                count++;
                if (layer.preview && layer.spriteTexture != null)
                {
                    Rect targetRect = rect;
                    targetRect.position -= (layer.positionOffset * 128);
                    if (layer.colorSource == CreatureSpriteLayer.ColorSource.None)
                        mat.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    else
                        mat.color = layer.color;
                    mat.SetVector(rectID,
                        new Vector4(layer.spriteTexture.rect.x / layer.spriteTexture.texture.width,
                        layer.spriteTexture.rect.y / layer.spriteTexture.texture.height,
                        layer.spriteTexture.rect.width / layer.spriteTexture.texture.width,
                        layer.spriteTexture.rect.height / layer.spriteTexture.texture.height));
                    EditorGUI.DrawPreviewTexture(targetRect, layer.spriteTexture.texture, mat, ScaleMode.StretchToFill);
                }
            }
        }
        maxLayers = EditorGUILayout.IntSlider(maxLayers, 0, collection.spriteLayers.Count);
        listPosition = EditorGUILayout.BeginScrollView(listPosition);
        list.DoLayoutList();
        EditorGUILayout.EndScrollView();
        serializedObject.ApplyModifiedProperties();
    }
}
