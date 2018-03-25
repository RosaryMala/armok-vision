using MaterialStore;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingMaterialSetter))]
public class BuildingMaterialSetterEditor : Editor
{
    MaterialCollection originalCollection;

    MaterialType type = MaterialType.NONE;
    List<MaterialType> typeList = new List<MaterialType>();
    string[] typeListLabels;
    int selectedType;

    string tag1;
    List<string> tag1List = new List<string>();
    string[] tag1Labels;
    int selectedTag1;

    string tag2;
    List<string> tag2List = new List<string>();
    string[] tag2Labels;
    int selectedTag2;

    List<MaterialTextureSet> validTextures = new List<MaterialTextureSet>();
    string[] materialList;
    int selectedMaterial;

    class MaterialReference
    {
        public MaterialReference(string tag, int index)
        {
            Tag = tag;
            Index = index;
        }

        public string Tag { get; private set; }
        public int Index { get; private set; }
    }

    void PopulateDropdowns(BuildingMaterialSetter setter)
    {
        typeList.Clear();
        typeList.Add(MaterialType.NONE);

        tag1List.Clear();
        tag2List.Clear();
        tag1List.Add("*");
        tag2List.Add("*");
        foreach (var item in originalCollection.textures)
        {
            var tag = item.tag;

            if (!typeList.Contains(tag.type)
                && (string.IsNullOrEmpty(tag1) || tag1 == "*" || tag1 == tag.tag1)
                && (string.IsNullOrEmpty(tag2) || tag2 == "*" || tag2 == tag.tag2))
                typeList.Add(tag.type);

            if (!tag1List.Contains(tag.tag1)
                &(type == MaterialType.NONE || type == tag.type)
                && (string.IsNullOrEmpty(tag2) || tag2 == "*" || tag2 == tag.tag2)
                && !string.IsNullOrEmpty(tag.tag1))
                tag1List.Add(tag.tag1);

            if (!tag2List.Contains(tag.tag2)
                & (type == MaterialType.NONE || type == tag.type)
                && (string.IsNullOrEmpty(tag1) || tag1 == "*" || tag1 == tag.tag1)
                && !string.IsNullOrEmpty(tag.tag2))
                tag2List.Add(tag.tag2);
        }

        typeListLabels = new string[typeList.Count];
        for(int i = 0; i < typeList.Count; i++)
        {
            typeListLabels[i] = typeList[i].ToString();
        }
        selectedType = typeList.FindIndex(x => x == type);
        selectedType = Mathf.Max(selectedType, 0);

        tag1Labels = new string[tag1List.Count];
        for (int i = 0; i < tag1List.Count; i++)
        {
            tag1Labels[i] = tag1List[i];
        }
        selectedTag1 = tag1List.FindIndex(x => x == tag1);
        selectedTag1 = Mathf.Max(selectedTag1, 0);

        tag2Labels = new string[tag2List.Count];
        for (int i = 0; i < tag2List.Count; i++)
        {
            tag2Labels[i] = tag2List[i];
        }
        selectedTag2 = tag2List.FindIndex(x => x == tag2);
        selectedTag2 = Mathf.Max(selectedTag2, 0);

        validTextures.Clear();
        foreach (var item in originalCollection.textures)
        {
            var tag = item.tag;
            if (type != MaterialType.NONE && type != tag.type)
                continue;
            if (tag1 != "*" && tag1 != tag.tag1)
                continue;
            if (tag2 != "*" && tag2 != tag.tag2)
                continue;
            validTextures.Add(item);
        }

        materialList = new string[validTextures.Count];
        if (validTextures.Count > 0)
        {
            for (int i = 0; i < validTextures.Count; i++)
            {
                materialList[i] = validTextures[i].tag.ToString();
            }
            selectedMaterial = validTextures.FindIndex(x => x.tag.Equals(setter.currentMaterialTag));
            selectedMaterial = Mathf.Max(selectedMaterial, 0);
            setter.SetMaterials(validTextures[selectedMaterial]);
        }
    }

    public override void OnInspectorGUI()
    {
        var setter = (BuildingMaterialSetter)target;
        var targetStore = Resources.Load<MaterialCollection>("materialDefinitions");
        if (materialList == null || originalCollection == null || targetStore != originalCollection)
        {
            originalCollection = targetStore;
            PopulateDropdowns(setter);
        }
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        var backupType = selectedType;
        selectedType = EditorGUILayout.Popup(selectedType, typeListLabels);
        if (selectedType != backupType)
        {
            type = typeList[selectedType];
            PopulateDropdowns(setter);
        }
        if (tag1Labels.Length > 1)
        {
            var backupTag1 = selectedTag1;
            selectedTag1 = EditorGUILayout.Popup(selectedTag1, tag1Labels);
            if (selectedTag1 != backupTag1)
            {
                tag1 = tag1List[selectedTag1];
                PopulateDropdowns(setter);
            }
        }
        if (tag2Labels.Length > 1)
        {
            var backupTag2 = selectedTag2;
            selectedTag2 = EditorGUILayout.Popup(selectedTag2, tag2Labels);
            if (selectedTag2 != backupTag2)
            {
                tag2 = tag2List[selectedTag2];
                PopulateDropdowns(setter);
            }
        }
        var backupMaterial = selectedMaterial;
        EditorGUILayout.EndHorizontal();
        if (validTextures.Count > 0)
        {
            selectedMaterial = EditorGUILayout.Popup(selectedMaterial, materialList);
            if (selectedMaterial != backupMaterial)
            {
                var mat = validTextures[selectedMaterial];
                setter.currentMaterialTag = mat.tag;
                setter.SetMaterials(mat);
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(setter);
        }
    }
}
