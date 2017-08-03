using MaterialStore;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingMaterialSetter))]
public class BuildingMaterialSetterEditor : Editor
{
    MaterialCollection originalCollection;
    string[] materialList;
    public override void OnInspectorGUI()
    {
        var setter = (BuildingMaterialSetter)target;
        setter.materialStore = Resources.Load<MaterialCollection>("materialDefinitions");
        var targetStore = setter.materialStore;
        if (materialList == null || originalCollection == null || targetStore != originalCollection)
        {
            originalCollection = targetStore;
            materialList = new string[targetStore.textures.Count];
            for(int i = 0; i < targetStore.textures.Count; i++)
            {
                materialList[i] = targetStore.textures[i].tag.ToString();
            }
            Debug.Log("Updated " + materialList.Length + " material names");
        }
        EditorGUI.BeginChangeCheck();
        setter.materialChosen = EditorGUILayout.Popup(setter.materialChosen, materialList);
        setter.materialChosen = EditorGUILayout.IntSlider(setter.materialChosen, 0, targetStore.textures.Count - 1);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(setter);
            setter.SetMaterials();
        }

    }
}
