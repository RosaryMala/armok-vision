using RemoteFortressReader;
using System.Collections.Generic;
using System.Xml.Linq;

abstract public class MaterialConfiguration<T> {
    MaterialMatcher<T> materialMatcher = new MaterialMatcher<T>();
    abstract public string rootName { get; }
    abstract protected string nodeName { get; }

    public T this[MatPairStruct mat]
    {
        get
        {
            return materialMatcher[mat];
        }
    }

    public List<MaterialDefinition> matTokenList
    {
        set
        {
            materialMatcher.matTokenList = value;
        }
    }
    public bool AddSingleMaterialConfig(XElement elemRoot)
    {
        if (elemRoot.Name != rootName)
            return true;
        var elemColors = elemRoot.Elements(nodeName);
        foreach (XElement elemColor in elemColors)
        {
            ParseMaterialElement(elemColor);
        }

        return true;
    }

    abstract public bool ParseTypeElement(XElement elemtype, out T value);

    void ParseMaterialElement(XElement elemtype)
    {
        T value;
        if (!ParseTypeElement(elemtype, out value))
        {
            //put an error message here
            return;
        }
        var elemMaterials = elemtype.Elements("material");
        foreach (XElement elemMaterial in elemMaterials)
        {
            XAttribute elemToken = elemMaterial.Attribute("token");
            if (elemToken != null)
            {
                materialMatcher[elemToken.Value] = value;
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
                materialMatcher[material] = value;
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
                    materialMatcher[token] = value;
                }
            }

        }

    }
}
