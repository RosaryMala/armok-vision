using MaterialStore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class MaterialXMLConverter
{
    [MenuItem("Mytools/Build Material Collection")]
    public static void BuildMaterialCollection()
    {
        var xmlFilePath = EditorUtility.OpenFilePanel("Pick an XML file to load", "Assets/StreamingAssets", "xml");

        XElement doc = XElement.Load(xmlFilePath, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);

        if(doc.Name.LocalName != "colors")
        {
            Debug.Log("Only colors available currently, quitting.");
        }

        var materialCollection = ScriptableObject.CreateInstance<MaterialCollection>();
        foreach (var item in doc.Elements())
        {
            Color32 color32 = new Color32();

            color32.r = byte.Parse(item.Attribute("red").Value);
            color32.g = byte.Parse(item.Attribute("green").Value);
            color32.b = byte.Parse(item.Attribute("blue").Value);

            XAttribute colorAlphaAtt = item.Attribute("metal");
            if (colorAlphaAtt != null)
            {
                switch (colorAlphaAtt.Value)
                {
                    case "yes":
                        color32.a = 255;
                        break;
                    case "no":
                        color32.a = 128;
                        break;
                    default:
                        color32.a = byte.Parse(colorAlphaAtt.Value);
                        break;
                }
            }
            else
                color32.a = 128;

            foreach (var mat in item.Elements("material"))
            {
                Debug.Log(mat.Attribute("token"));
                MaterialTag tag;
                if (!MaterialTag.TryParse(mat.Attribute("token").Value, out tag))
                    continue;

                MaterialTextureSet set = new MaterialTextureSet();
                set.color = color32;
                set.tag = tag;
                materialCollection.textures.Add(set);
            }
        }
        string assetPath = @"Assets\Resources\MaterialSets\" + Path.GetFileNameWithoutExtension(xmlFilePath) + ".asset";
        Debug.Log(assetPath);
        AssetDatabase.CreateAsset(materialCollection, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
