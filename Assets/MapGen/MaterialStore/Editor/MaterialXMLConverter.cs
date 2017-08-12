using MaterialStore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using System;

public class MaterialXMLConverter
{
    [MenuItem("Mytools/Convert XML to internal")]
    public static void BuildMaterialCollection()
    {
        var xmlFilePath = EditorUtility.OpenFilePanel("Pick an XML file to load", "Assets/StreamingAssets", "xml");

        XElement doc = XElement.Load(xmlFilePath, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);

        switch (doc.Name.LocalName)
        {
            case "colors":
                ConvertColorXML(doc);
                break;
            case "creatures":
                ConvertCreatureXML(doc);
                break;
            default:
                break;
        }


    }

    private static void ConvertCreatureXML(XElement doc)
    {
        foreach (var creature in doc.Elements())
        {
            string race = creature.Attribute("gameID").Value;
            Debug.Log(race);
            foreach (var variant in creature.Elements("variant"))
            {
                CreatureSpriteCollection spriteCollection = ScriptableObject.CreateInstance<CreatureSpriteCollection>();

                spriteCollection.race = race;
                var special = variant.Attribute("special");
                if (special == null)
                    spriteCollection.special = CreatureSpriteCollection.Special.Normal;
                else
                    spriteCollection.special = (CreatureSpriteCollection.Special)Enum.Parse(typeof(CreatureSpriteCollection.Special), special.Value);

                var sex = variant.Attribute("sex");
                if (sex != null)
                {
                    switch (sex.Value)
                    {
                        case "M":
                            spriteCollection.caste = "MALE";
                            break;
                        case "F":
                            spriteCollection.caste = "FEMALE";
                            break;
                        default:
                            spriteCollection.caste = sex.Value;
                            break;
                    }
                }

                var prof = variant.Attribute("prof");
                if(prof != null)
                {
                    spriteCollection.profession = prof.Value;
                }

                string filePrefix = Path.GetFileNameWithoutExtension(variant.Attribute("file").Value);

                spriteCollection.spriteLayers = new List<CreatureSpriteLayer>();

                foreach (var subsprite in variant.Elements("subsprite"))
                {
                    var sheetIndex = subsprite.Attribute("sheetIndex");
                    if (sheetIndex == null)
                        continue;
                    int index = int.Parse(sheetIndex.Value);

                    var file = subsprite.Attribute("file");
                    string spriteName = filePrefix + "-" + index;

                    if (file != null)
                        spriteName = Path.GetFileNameWithoutExtension(file.Value) + "-" + index;

                    var matchingSprites = AssetDatabase.FindAssets(spriteName + " t:Sprite");
                    if(matchingSprites == null || matchingSprites.Length == 0)
                    {
                        Debug.LogWarning("Could not find any sprite named " + spriteName);
                        continue;
                    }

                    CreatureSpriteLayer layer = new CreatureSpriteLayer();

                    layer.spriteTexture = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(matchingSprites[0]));
                    layer.preview = true;
                    layer.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

                    var colorSelection = subsprite.Attribute("color");
                    if(colorSelection != null)
                    {
                        switch (colorSelection.Value)
                        {
                            case "bodypart":
                                layer.spriteSource = CreatureSpriteLayer.SpriteSource.Bodypart;
                                var bodypart = subsprite.Attribute("bodypart");
                                if (bodypart != null)
                                    layer.token = bodypart.Value;
                                break;
                            case "equipment":
                                layer.spriteSource = CreatureSpriteLayer.SpriteSource.Equipment;
                                var equipment_name = subsprite.Attribute("equipment_name");
                                if (equipment_name != null)
                                    layer.token = equipment_name.Value;
                                break;
                            default:
                                break;
                        }
                    }

                    spriteCollection.spriteLayers.Add(layer);
                }

                Directory.CreateDirectory("Assets/Resources/Creatures");
                AssetDatabase.CreateAsset(spriteCollection, "Assets/Resources/Creatures/" + spriteCollection.race + "-" + spriteCollection.caste + "-" + spriteCollection.special + "-" + spriteCollection.profession + ".asset");
            }
        }
        AssetDatabase.Refresh();
    }

    private static void ConvertColorXML(XElement doc)
    {
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

                string subFolder = Path.GetFileNameWithoutExtension(doc.BaseUri) + "/";
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
