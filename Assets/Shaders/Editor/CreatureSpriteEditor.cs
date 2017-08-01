using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CreatureSpriteEditor : ShaderGUI
{
    enum Layers
    {
        layer_1, layer_2, layer_4, layer_8, layer_16, layer_32
    }

    string[] layerNames = new string[] { "layer_1", "layer_2", "layer_4", "layer_8", "layer_16", "layer_32" };

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
        // get the current keywords from the material
        Material targetMat = materialEditor.target as Material;
        string[] keyWords = targetMat.shaderKeywords;

        int currentKeyword = -1;
        for (int i = 0; i < layerNames.Length; i++)
        {
            if (keyWords.Contains(layerNames[i]))
            {
                currentKeyword = i;
                break;
            }
        }
        if(currentKeyword < 0)
        {
            currentKeyword = 0;
            targetMat.EnableKeyword(layerNames[currentKeyword]);
        }
        EditorGUI.BeginChangeCheck();
        int newKeyword = EditorGUILayout.Popup(currentKeyword, layerNames);
        if(EditorGUI.EndChangeCheck())
        {
            targetMat.DisableKeyword(layerNames[currentKeyword]);
            targetMat.EnableKeyword(layerNames[newKeyword]);
            EditorUtility.SetDirty(targetMat);
        }
    }
}