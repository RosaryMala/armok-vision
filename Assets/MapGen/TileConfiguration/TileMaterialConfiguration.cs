using RemoteFortressReader;
using System.Xml.Linq;

public class TileMaterialConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    const int PlantType = 419;
    MaterialMatcher<T> thisMaterialMatcher;
    MaterialMatcher<Content> materialMatcher = new MaterialMatcher<Content>();
    Content defaultMaterial;

    public override object SecondaryDictionary
    {
        set
        {
            thisMaterialMatcher = value as MaterialMatcher<T>;
        }
    }

    bool GetMaterialRef(MatPairStruct material, MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        Content cont;
        if (materialMatcher.TryGetValue(material, out cont))
        {
            if(thisMaterialMatcher == null)
            {
                value = cont.GetValue(tile, layer);
                return true;
            }
            else if (cont.overloadedItem != null)
            {
                if (cont.overloadedItem.GetValue(tile, layer, out value))
                    return true;
            }
        }
        else if (thisMaterialMatcher != null && thisMaterialMatcher.TryGetValue(material, out value))
        {
            return true;
        }
        value = default(T);
        return false;
    }

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        MatPairStruct mat = tile.material;
        int plantIndex = mat.mat_index;

        switch (layer)
        {
            case MeshLayer.StaticMaterial:
            case MeshLayer.StaticCutout:
            case MeshLayer.StaticTransparent:
                return GetMaterialRef(tile.material, tile, layer, out value);
            case MeshLayer.BaseMaterial:
            case MeshLayer.BaseCutout:
            case MeshLayer.BaseTransparent:
                return GetMaterialRef(tile.base_material, tile, layer, out value);
            case MeshLayer.LayerMaterial:
            case MeshLayer.LayerCutout:
            case MeshLayer.LayerTransparent:
                return GetMaterialRef(tile.layer_material, tile, layer, out value);
            case MeshLayer.VeinMaterial:
            case MeshLayer.VeinCutout:
            case MeshLayer.VeinTransparent:
                return GetMaterialRef(tile.vein_material, tile, layer, out value);
            case MeshLayer.NoMaterial:
            case MeshLayer.NoMaterialCutout:
            case MeshLayer.NoMaterialBuildingCutout:
            case MeshLayer.NoMaterialBuilding:
            case MeshLayer.NoMaterialBuildingTransparent:
            case MeshLayer.NoMaterialTransparent:
                if (defaultMaterial == null)
                    break;
                value = defaultMaterial.GetValue(tile, layer);
                return true;
            case MeshLayer.BuildingMaterial:
            case MeshLayer.BuildingMaterialCutout:
            case MeshLayer.BuildingMaterialTransparent:
                return GetMaterialRef(tile.buildingMaterial, tile, layer, out value);
            case MeshLayer.GrowthMaterial:
            case MeshLayer.GrowthCutout:
            case MeshLayer.GrowthTransparent:
                if ((mat.mat_type != PlantType)
                    || DFConnection.Instance.NetPlantRawList == null
                    || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= plantIndex
                    || DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths.Count <= 0
                    || DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths[0].mat == null)
                {
                    value = default(T);
                    return false;
                }
                return GetMaterialRef(DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths[0].mat, tile, layer, out value);
            case MeshLayer.GrowthMaterial1:
            case MeshLayer.GrowthCutout1:
            case MeshLayer.GrowthTransparent1:
                if ((mat.mat_type != PlantType)
                    || DFConnection.Instance.NetPlantRawList == null
                    || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= plantIndex
                    || DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths.Count <= 1
                    || DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths[0].mat == null)
                {
                    value = default(T);
                    return false;
                }
                return GetMaterialRef(DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths[1].mat, tile, layer, out value);
            case MeshLayer.GrowthMaterial2:
            case MeshLayer.GrowthCutout2:
            case MeshLayer.GrowthTransparent2:
                if ((mat.mat_type != PlantType)
                    || DFConnection.Instance.NetPlantRawList == null
                    || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= plantIndex
                    || DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths.Count <= 2
                    || DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths[0].mat == null)
                {
                    value = default(T);
                    return false;
                }
                return GetMaterialRef(DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths[2].mat, tile, layer, out value);
            case MeshLayer.GrowthMaterial3:
            case MeshLayer.GrowthCutout3:
            case MeshLayer.GrowthTransparent3:
                if ((mat.mat_type != PlantType)
                    || DFConnection.Instance.NetPlantRawList == null
                    || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= plantIndex
                    || DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths.Count <= 3
                    || DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths[0].mat == null)
                {
                    value = default(T);
                    return false;
                }
                return GetMaterialRef(DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths[3].mat, tile, layer, out value);
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
                {
                    if (content.overloadedItem != null || thisMaterialMatcher == null)
                        materialMatcher[elemToken.Value] = content;
                    if (thisMaterialMatcher != null)
                        thisMaterialMatcher[elemToken.Value] = content.defaultItem;
                }
                continue;
            }

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
                //handle here the elements without subtypes.
                MatPairStruct material = new MatPairStruct((int)elemType, -1);
                if (content.overloadedItem != null || thisMaterialMatcher == null)
                    materialMatcher[material] = content;
                if (thisMaterialMatcher != null)
                    thisMaterialMatcher[material] = content.defaultItem;
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
                    if (content.overloadedItem != null || thisMaterialMatcher == null)
                        materialMatcher[token] = content;
                    if (thisMaterialMatcher != null)
                        thisMaterialMatcher[token] = content.defaultItem;
                }
            }
        }
    }
}
