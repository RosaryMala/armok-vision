using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitMaker))]
public class UnitMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Read from DF"))
        {

        }
        base.OnInspectorGUI();
    }
}
