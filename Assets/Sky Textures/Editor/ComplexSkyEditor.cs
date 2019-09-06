using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ComplexSkyEditor : ShaderGUI
{
    private MaterialProperty[] properties;
    private Material material;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.properties = properties;
        material = (Material)materialEditor.target;
        EditorGUI.BeginChangeCheck();
        base.OnGUI(materialEditor, properties);
        EditorGUI.EndChangeCheck();
        //if (GUI.changed)
        {
            MaterialProperty starRotation = FindProperty("_StarRotation");
            var rot = starRotation.vectorValue;
            var rotQat = Quaternion.Euler(rot);
            var matrix = Matrix4x4.Rotate(rotQat);
            material.SetMatrix("_StarRotationMatrix", matrix);
            starRotation = FindProperty("_MoonRotation");
            rot = starRotation.vectorValue;
            rotQat = Quaternion.Euler(rot);
            matrix = Matrix4x4.Rotate(rotQat);
            material.SetMatrix("_MoonRotationMatrix", matrix);
        }
    }

    MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, properties);
    }
}
