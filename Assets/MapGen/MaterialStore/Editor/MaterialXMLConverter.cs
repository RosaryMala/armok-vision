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

            foreach (var elemMaterial in item.Elements("material"))
            {
                MaterialTag tag = new MaterialTag();
                XAttribute elemToken = elemMaterial.Attribute("token");
                if (elemToken != null)
                {
                    if (!MaterialTag.TryParse(elemMaterial.Attribute("token").Value, out tag))
                        continue;
                }
                else
                {
                    //if there's no material token, fall back to the out-dated system.
                    XAttribute elemValue = elemMaterial.Attribute("value");
                    MatBasic elemType = MatBasic.INVALID;
                    if (elemValue != null)
                    {
                        elemType = ContentLoader.lookupMaterialType(elemValue.Value);
                    }
                    if (elemType == MatBasic.INVALID)
                    {
                        //throw error here
                        continue;
                    }

                    //parse subtype elements
                    if (elemMaterial.Element("subtype") == null)
                    {
                        tag.SetBasic(elemType);
                    }
                    else
                    {
                        var elemSubtypes = elemMaterial.Elements("subtype");
                        foreach (XElement elemSubtype in elemSubtypes)
                        {
                            XAttribute subtypeValue = elemSubtype.Attribute("value");
                            if (subtypeValue == null)
                            {
                                //Oh no!
                                continue;
                            }
                            string subtype = subtypeValue.Value;
                            string token = "";
                            switch (elemType)
                            {
                                case MatBasic.INORGANIC:
                                    token = "INORGANIC:" + subtype;
                                    break;
                                case MatBasic.ICHOR:
                                    token = "CREATURE:" + subtype + ":ICHOR";
                                    break;
                                case MatBasic.LEATHER:
                                    token = "CREATURE:" + subtype + ":LEATHER";
                                    break;
                                case MatBasic.BLOOD_1:
                                    token = "CREATURE:" + subtype + ":BLOOD";
                                    break;
                                case MatBasic.BLOOD_2:
                                    token = "CREATURE:" + subtype + ":BLOOD";
                                    break;
                                case MatBasic.BLOOD_3:
                                    token = "CREATURE:" + subtype + ":BLOOD";
                                    break;
                                case MatBasic.BLOOD_4:
                                    token = "CREATURE:" + subtype + ":BLOOD";
                                    break;
                                case MatBasic.BLOOD_5:
                                    token = "CREATURE:" + subtype + ":BLOOD";
                                    break;
                                case MatBasic.BLOOD_6:
                                    token = "CREATURE:" + subtype + ":BLOOD";
                                    break;
                                case MatBasic.PLANT:
                                    token = "PLANT:" + subtype + ":STRUCTURAL";
                                    break;
                                case MatBasic.WOOD:
                                    token = "PLANT:" + subtype + ":WOOD";
                                    break;
                                case MatBasic.PLANTCLOTH:
                                    token = "PLANT:" + subtype + ":THREAD";
                                    break;
                                default:
                                    //make some kind of error here
                                    continue;
                            }
                            MaterialTag.TryParse(token, out tag);
                        }
                    }
                }
                Debug.Log(tag);

                string subFolder = Path.GetFileNameWithoutExtension(xmlFilePath) + "/";
                string assetPath = "Assets/Resources/Materials/" + subFolder + tag.ToFileName() + ".mat";
                var saveMat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (saveMat == null)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(assetPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
                    saveMat = new Material(Shader.Find("Custom/MaterialStore"));
                    AssetDatabase.CreateAsset(saveMat, assetPath);
                }
                saveMat.SetColor("_Color", color32);

                Debug.Log(assetPath);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
