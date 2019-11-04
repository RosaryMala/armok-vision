using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(CreatureBody))]
public class CreatureBodyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Dump mods"))
        {
            var path = EditorUtility.SaveFilePanel("Save mod tree", "", "dwarf.txt", "txt");
            if(!string.IsNullOrWhiteSpace(path))
            {
                CreatureBody body = target as CreatureBody;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                {
                    int indent = 0;
                    WritePartMods(file, body.rootPart, indent);
                }
            }
        }
    }

    private void WritePartMods(StreamWriter file, BodyPart rootPart, int indent)
    {
        file.WriteLine(new string(' ', indent) + rootPart.category);
        foreach (var mod in rootPart.mods)
        {
            file.WriteLine(new string(' ', indent) + "-" + mod.type);
        }
        foreach (var layer in rootPart.layerModels)
        {
            if (layer.Mods.Count == 0)
                continue;
            file.WriteLine(new string(' ', indent) + ">" + layer.RawLayerName);
            foreach (var mod in layer.Mods)
            {
                file.WriteLine(new string(' ', indent) + ">-" + mod.type);
            }
        }
        foreach (var child in rootPart.children)
        {
            WritePartMods(file, child, indent + 4);
        }
    }
}
