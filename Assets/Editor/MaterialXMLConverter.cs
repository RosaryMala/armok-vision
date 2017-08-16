using MaterialStore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using System;
using DF.Enums;

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
            var creatureFile = creature.Attribute("file");
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
                    spriteCollection.profession = (profession)Enum.Parse((typeof(profession)), prof.Value);
                }

                var variantFile = variant.Attribute("file");

                spriteCollection.spriteLayers = new List<CreatureSpriteLayer>();

                foreach (var subsprite in variant.Elements("subsprite"))
                {
                    var sheetIndex = subsprite.Attribute("sheetIndex");
                    if (sheetIndex == null)
                        continue;
                    int index = int.Parse(sheetIndex.Value);

                    string spriteName;

                    var layerFile = subsprite.Attribute("file");
                    if (layerFile != null)
                        spriteName = Path.GetFileNameWithoutExtension(layerFile.Value) + "-" + index;
                    else if (variantFile != null)
                        spriteName = Path.GetFileNameWithoutExtension(variantFile.Value) + "-" + index;
                    else if (creatureFile != null)
                        spriteName = Path.GetFileNameWithoutExtension(creatureFile.Value) + "-" + index;
                    else
                    {
                        Debug.LogError("Could not find matching file");
                        continue;
                    }

                    var matchingSprites = AssetDatabase.FindAssets(spriteName + " t:Sprite");
                    if(matchingSprites == null || matchingSprites.Length == 0)
                    {
                        Debug.LogWarning("Could not find any sprite named " + spriteName);
                        continue;
                    }

                    CreatureSpriteLayer layer = new CreatureSpriteLayer();

                    layer.spriteTexture = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(matchingSprites[0]));
                    layer.preview = true;

                    Color32 color = new Color32(128, 128, 128, 128);

                    foreach (var attribute in subsprite.Attributes())
                    {
                        switch (attribute.Name.LocalName)
                        {
                            case "sheetIndex":
                            case "file":
                                break; //already taken care of
                            case "zoom":
                            case "equipment_class":
                                break; //not used
                            case "color":
                                switch (attribute.Value)
                                {
                                    case "bodypart":
                                        layer.spriteSource = CreatureSpriteLayer.SpriteSource.Bodypart;
                                        layer.colorSource = CreatureSpriteLayer.ColorSource.Material;
                                        break;
                                    case "equipment":
                                        layer.spriteSource = CreatureSpriteLayer.SpriteSource.Equipment;
                                        layer.colorSource = CreatureSpriteLayer.ColorSource.Material;
                                        break;
                                    case "xml":
                                        layer.spriteSource = CreatureSpriteLayer.SpriteSource.Static;
                                        layer.colorSource = CreatureSpriteLayer.ColorSource.Fixed;
                                        break;
                                    default:
                                        Debug.LogError("Unknown creature sprite layer color attribute: " + attribute);
                                        break;
                                }
                                break;
                            case "bodypart":
                            case "equipment_name":
                                layer.token = attribute.Value;
                                break;
                            case "red":
                                color.r = byte.Parse(attribute.Value);
                                break;
                            case "green":
                                color.g = byte.Parse(attribute.Value);
                                break;
                            case "blue":
                                color.b = byte.Parse(attribute.Value);
                                break;
                            case "alpha":
                                color.a = (byte)(byte.Parse(attribute.Value) / 2);
                                break;
                            case "pattern_index":
                                layer.patternIndex = int.Parse(attribute.Value);
                                break;
                            case "hair_min":
                                layer.hairMin = int.Parse(attribute.Value);
                                break;
                            case "hair_max":
                                layer.hairMax = int.Parse(attribute.Value);
                                break;
                            case "hair_type":
                                switch (attribute.Value)
                                {
                                    case "hair":
                                        layer.hairType = CreatureSpriteLayer.HairType.Hair;
                                        break;
                                    case "sideburns":
                                        layer.hairType = CreatureSpriteLayer.HairType.Sideburns;
                                        break;
                                    case "beard":
                                        layer.hairType = CreatureSpriteLayer.HairType.Beard;
                                        break;
                                    case "moustache":
                                        layer.hairType = CreatureSpriteLayer.HairType.Moustache;
                                        break;
                                    default:
                                        layer.hairType = CreatureSpriteLayer.HairType.None;
                                        break;
                                }
                                break;
                            case "hair_style":
                                switch (attribute.Value)
                                {
                                    case "unkempt":
                                        layer.hairStyle = RemoteFortressReader.HairStyle.UNKEMPT;
                                        break;
                                    case "combed":
                                        layer.hairStyle = RemoteFortressReader.HairStyle.NEATLY_COMBED;
                                        break;
                                    case "braid":
                                        layer.hairStyle = RemoteFortressReader.HairStyle.BRAIDED;
                                        break;
                                    case "two_braid":
                                        layer.hairStyle = RemoteFortressReader.HairStyle.DOUBLE_BRAID;
                                        break;
                                    case "ponytails":
                                        layer.hairStyle = RemoteFortressReader.HairStyle.PONY_TAILS;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "material":
                                switch (attribute.Value)
                                {
                                    case "Metal":
                                        layer.metal = true;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "offsety":
                                layer.positionOffset.y = float.Parse(attribute.Value) / -16.0f;
                                break;
                            case "offsetx":
                                layer.positionOffset.x = float.Parse(attribute.Value) / -16.0f;
                                break;
                            default:
                                Debug.LogError("Unknown creature sprite layer attribute: " + attribute);
                                break;
                        }
                    }

                    layer.color = color;

                    spriteCollection.spriteLayers.Add(layer);
                }

                Directory.CreateDirectory("Assets/Resources/Creatures/" + spriteCollection.race + "/");
                AssetDatabase.CreateAsset(spriteCollection, "Assets/Resources/Creatures/" + spriteCollection.race + "/" + spriteCollection.caste + "-" + spriteCollection.special + "-" + spriteCollection.profession + ".asset");
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
