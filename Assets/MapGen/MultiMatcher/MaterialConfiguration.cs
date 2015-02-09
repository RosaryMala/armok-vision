using System.Xml.Linq;

public class MaterialConfiguration<T> : ContentConfiguration<T> where T : IContent, new()
{
    MaterialMatcher<Content> materialMatcher = new MaterialMatcher<Content>();

    public override bool GetValue(MapTile tile, out T value)
    {
        Content cont;
        if (materialMatcher.Get(tile.material, out cont))
        {
            value = cont.GetValue(tile);
            return true;
        }
        value = default(T);
        return false;
    }

    protected override void ParseElementConditions(XElement elemtype, Content content)
    {
        var elemMaterials = elemtype.Elements("material");
        foreach (XElement elemMaterial in elemMaterials)
        {
            XAttribute elemToken = elemMaterial.Attribute("token");
            if (elemToken != null)
            {
                materialMatcher[elemToken.Value] = content;
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
                materialMatcher[material] = content;
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
                    materialMatcher[token] = content;
                }
            }
        }
    }
}
