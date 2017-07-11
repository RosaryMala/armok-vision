using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class MaterialXMLConverter
{
    [MenuItem("Mytools/Build Material Collection")]
    public static void BuildMaterialCollection()
    {
        var xmlFilePath = EditorUtility.OpenFilePanel("Pick an XML file to load", "", "xml");

        XElement doc = XElement.Load(xmlFilePath, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);



    }
}
