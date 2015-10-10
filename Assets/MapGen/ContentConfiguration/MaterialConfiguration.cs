using System.Xml.Linq;

public class MaterialConfiguration<T> : ContentConfiguration<T> where T : IContent, new()
{
    MaterialMatcher<Content> materialMatcher = new MaterialMatcher<Content>();
    Content defaultMaterial;
    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        Content cont;
        switch (layer)
        {
            case MeshLayer.StaticMaterial:
            case MeshLayer.StaticCutout:
                if (materialMatcher.Get(tile.material, out cont))
                {
                    value = cont.GetValue(tile, layer);
                    return true;
                }
                break;
            case MeshLayer.BaseMaterial:
            case MeshLayer.BaseCutout:
                if (materialMatcher.Get(tile.base_material, out cont))
                {
                    value = cont.GetValue(tile, layer);
                    return true;
                }
                break;
            case MeshLayer.LayerMaterial:
            case MeshLayer.LayerCutout:
                if (materialMatcher.Get(tile.layer_material, out cont))
                {
                    value = cont.GetValue(tile, layer);
                    return true;
                }
                break;
            case MeshLayer.VeinMaterial:
            case MeshLayer.VeinCutout:
                if (materialMatcher.Get(tile.vein_material, out cont))
                {
                    value = cont.GetValue(tile, layer);
                    return true;
                }
                break;
            case MeshLayer.NoMaterial:
            case MeshLayer.NoMaterialCutout:
            case MeshLayer.NoMaterialBuildingCutout:
            case MeshLayer.NoMaterialBuilding:
                if (defaultMaterial == null)
                    break;
                value = defaultMaterial.GetValue(tile, layer);
                return true;
            case MeshLayer.Growth0Cutout:
                break;
            case MeshLayer.Growth1Cutout:
                break;
            case MeshLayer.Growth2Cutout:
                break;
            case MeshLayer.Growth3Cutout:
                break;
            case MeshLayer.BuildingMaterial:
            case MeshLayer.BuildingMaterialCutout:
                if (materialMatcher.Get(tile.buildingMaterial, out cont))
                {
                    value = cont.GetValue(tile, layer);
                    return true;
                }
                break;
            default:
                break;
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
                if (elemToken.Value == "NONE")
                    defaultMaterial = content;
                else
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
