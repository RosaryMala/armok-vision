using System;
using System.Xml.Linq;

public class TileMaterialConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
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
        if (materialMatcher.Get(material, out cont))
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
        else if (thisMaterialMatcher != null && thisMaterialMatcher.Get(material, out value))
        {
            return true;
        }
        value = default(T);
        return false;
    }

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
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
            case MeshLayer.BuildingMaterialTransparent:
                return GetMaterialRef(tile.buildingMaterial, tile, layer, out value);
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
