using System.Xml.Linq;
using UnityEngine;
using System.Collections.Generic;
using RemoteFortressReader;

public class ColorConfiguration
{
    MaterialMatcher<Color> colorMatcher = new MaterialMatcher<Color>();

    public Color this[MatPairStruct mat]
    {
        get
        {
            return colorMatcher[mat];
        }
    }

    public List<MaterialDefinition> matTokenList
    {
        set
        {
            colorMatcher.matTokenList = value;
        }
    }
    public bool AddSingleColorConfig(XElement elemRoot)
    {
        if (elemRoot.Name != "colors")
            return true;
        var elemColors = elemRoot.Elements("color");
        foreach (XElement elemColor in elemColors)
        {
            ParseColorElement(elemColor);           
        }

        return true;
    }

    void ParseColorElement(XElement elemColor)
    {
        var colorRedAtt = elemColor.Attribute("red");
        if (colorRedAtt == null)
        {
            //Add error message here
            return;
        }
        var colorGreenAtt = elemColor.Attribute("green");
        if (colorGreenAtt == null)
        {
            //Add error message here
            return;
        }
        var colorBlueAtt = elemColor.Attribute("blue");
        if (colorBlueAtt == null)
        {
            //Add error message here
            return;
        }
        int alpha = 255;
        var colorAlphaAtt = elemColor.Attribute("alpha");
        if (colorAlphaAtt != null)
        {
            int.TryParse(colorAlphaAtt.Value, out alpha);
        }
        int red, green, blue;
        int.TryParse(colorRedAtt.Value, out red);
        int.TryParse(colorGreenAtt.Value, out green);
        int.TryParse(colorBlueAtt.Value, out blue);
        Color color = new Color(red / 255.0f, green / 255.0f, blue / 255.0f, alpha / 255.0f);

        var elemMaterials = elemColor.Elements("material");
        foreach (XElement elemMaterial in elemMaterials)
        {
            XAttribute elemToken = elemMaterial.Attribute("token");
            if (elemToken != null)
            {
                colorMatcher[elemToken.Value] = color;
                continue;
            }

            //if there's no material token, fall back to the out-dated system.
            XAttribute elemValue = elemMaterial.Attribute("value");
            MatBasic elemIndex = MatBasic.INVALID;
            if (elemValue != null)
            {
                elemIndex = ContentLoader.lookupMaterialType(elemValue.Value);
            }
            if (elemIndex == MatBasic.INVALID)
            {
                //throw error here
                continue;
            }

            //parse subtype elements
            if (elemMaterial.Element("subtype") == null)
            {
                //handle here the elements without subtypes.
                MatPairStruct material;
                material.mat_index = (int)elemIndex;
                material.mat_type = -1;
                colorMatcher[material] = color;
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
                    switch (elemIndex)
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
                    colorMatcher[token] = color;
                }
            }

        }

    }
}
