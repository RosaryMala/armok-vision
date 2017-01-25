using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

[CustomEditor(typeof(MeshSerializer))]
public class MeshSerializerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Serialize XML"))
        {
            MeshSerializer tar = (MeshSerializer)target;

            if (tar != null && tar.outputMesh != null)
            {
                var outputMesh = tar.outputMesh;

                var path = EditorUtility.SaveFilePanel("Save mesh as XML", "", "out.xml", "xml");
                if (!string.IsNullOrEmpty(path))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(MeshContentSerialized));

                    TextWriter textOut = new StreamWriter(path);

                    ser.Serialize(textOut, outputMesh);
                }
            }
        }
        if (GUILayout.Button("Serialize Binary"))
        {
            MeshSerializer tar = (MeshSerializer)target;

            if (tar != null && tar.outputMesh != null)
            {
                var outputMesh = tar.outputMesh;

                var path = EditorUtility.SaveFilePanel("Save mesh as Binary avmesh", "", "out.avmesh", "avmesh");
                if (!string.IsNullOrEmpty(path))
                {
                    IFormatter formatter = new BinaryFormatter();

                    Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

                    formatter.Serialize(stream, outputMesh);
                }
            }
        }
    }
}
