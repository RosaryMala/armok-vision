using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MaterialRaws))]
public class MaterialRawEditor : Editor
{
    bool listExpanded = false;
    string filter;

    public override void OnInspectorGUI()
    {
        var raws = (MaterialRaws)target;

        filter = GUILayout.TextField(filter);
        listExpanded = EditorGUILayout.Foldout(listExpanded, "Material List");
        if(listExpanded)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                GUILayout.Label("You really need to filter this list");
            }
            else
            {
                foreach (var raw in raws.MaterialList)
                {
                    if (raw.id.ToUpper().Contains(filter.ToUpper()))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(raw.id);
                        EditorGUILayout.ColorField(raw.state_color);
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }
    }
}
