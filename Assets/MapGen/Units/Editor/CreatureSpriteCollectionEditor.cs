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
    private int maxLayers;
    private Vector2 wholePositionOffset = Vector2.zero;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("spriteLayers"), true, true, true, true);
        raceProp = serializedObject.FindProperty("race");
        casteProp = serializedObject.FindProperty("caste");
        professionProp = serializedObject.FindProperty("profession");
        specialProp = serializedObject.FindProperty("special");

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
                new Rect(rect.x + rect.width - 100, rect.y, 100, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("positionOffset"), GUIContent.none);
            rect.xMax -= 100;
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

    public static Rect getSpriteRect(Sprite sprite, Vector2 origin)
    {
        Rect spriteRect = sprite.rect;
        spriteRect.position -= new Vector2(spriteRect.width, spriteRect.height);
        spriteRect.position += origin;
        spriteRect.position += sprite.pivot;
        return spriteRect;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var collection = (CreatureSpriteCollection)target;
        EditorGUILayout.PropertyField(raceProp);
        EditorGUILayout.PropertyField(casteProp);
        EditorGUILayout.PropertyField(professionProp);
        EditorGUILayout.PropertyField(specialProp);
        EditorGUI.BeginChangeCheck();
        collection.standardSize = EditorGUILayout.DelayedFloatField("Standard Size", collection.standardSize);
        if (EditorGUI.EndChangeCheck())
        {
            collection.standardLength = Mathf.Pow(collection.standardSize, 1.0f / 3.0f) * 10;
            collection.standardArea = Mathf.Pow(collection.standardSize, 2.0f / 3.0f);
        }
        EditorGUI.BeginChangeCheck();
        collection.standardArea = EditorGUILayout.DelayedFloatField("Standard Area", collection.standardArea);
        if (EditorGUI.EndChangeCheck())
        {
            collection.standardLength = Mathf.Pow(collection.standardArea, 1.0f / 2.0f) * 10;
            collection.standardSize = Mathf.Pow(collection.standardArea, 3.0f / 2.0f);
        }
        EditorGUI.BeginChangeCheck();
        collection.standardLength = EditorGUILayout.DelayedFloatField("Standard Length", collection.standardLength);
        if (EditorGUI.EndChangeCheck())
        {
            collection.standardArea = Mathf.Pow(collection.standardLength / 10, 2.0f);
            collection.standardSize = Mathf.Pow(collection.standardLength / 10, 3.0f);
        }
        wholePositionOffset = EditorGUILayout.Vector2Field("Offset all sprites", wholePositionOffset);
        if (collection.spriteLayers != null && collection.spriteLayers.Count > 0)
        {
            if(wholePositionOffset.sqrMagnitude > 0)
            {
                if(GUILayout.Button("Apply"))
                {
                    foreach (var item in collection.spriteLayers)
                    {
                        item.positionOffset += wholePositionOffset;
                    }
                    wholePositionOffset = Vector2.zero;
                    EditorUtility.SetDirty(target);
                }
            }
            if (GUILayout.Button("Disable all"))
            {
                foreach (var layer in collection.spriteLayers)
                {
                    layer.preview = false;
                    EditorUtility.SetDirty(target);
                }
            }
            var rect = EditorGUILayout.GetControlRect(false, 256);
            Vector2 origin = new Vector2(rect.xMin + rect.xMax / 2, rect.yMax);
            //rect.width = rect.height;
            int count = 0;
            if (wholePositionOffset.sqrMagnitude > 0)
            {
                EditorGUI.DrawRect(new Rect(origin.x - 5 + (wholePositionOffset.x * 128), origin.y - 5 + (wholePositionOffset.y * 128), 10, 10), Color.yellow);
            }
            foreach (var layer in collection.spriteLayers)
            {
                if (count > maxLayers)
                    break;
                count++;
                if (layer.preview && layer.spriteTexture != null)
                {
                    Rect targetRect = getSpriteRect(layer.spriteTexture, origin);
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
            EditorGUI.DrawRect(new Rect(origin.x - 5, origin.y - 5, 10, 10), Color.magenta);
            maxLayers = EditorGUILayout.IntSlider(maxLayers, 0, collection.spriteLayers.Count);
        }
        listPosition = EditorGUILayout.BeginScrollView(listPosition);
        list.DoLayoutList();
        EditorGUILayout.EndScrollView();
        serializedObject.ApplyModifiedProperties();
    }
}
