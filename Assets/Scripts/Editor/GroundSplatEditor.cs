using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class GroundSplatEditor : MaterialEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!isVisible)
            return;

        // get the current keywords from the material
        Material targetMat = target as Material;
        string[] keyWords = targetMat.shaderKeywords;

        // see if redify is set, then show a checkbox
        bool contaminants = keyWords.Contains("CONTAMINANTS");
        EditorGUI.BeginChangeCheck();
        contaminants = EditorGUILayout.Toggle("Enable Contaminants Splatter", contaminants);
        if (EditorGUI.EndChangeCheck())
        {
            if (contaminants)
                targetMat.EnableKeyword("CONTAMINANTS");
            else
                targetMat.DisableKeyword("CONTAMINANTS");

            EditorUtility.SetDirty(targetMat);
        }
    }
}
