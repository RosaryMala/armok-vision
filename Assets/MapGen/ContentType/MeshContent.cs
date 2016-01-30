using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityExtension;

public enum MeshLayer
{
    StaticMaterial,
    BaseMaterial,
    LayerMaterial,
    VeinMaterial,
    GrowthMaterial,
    GrowthMaterial1,
    GrowthMaterial2,
    GrowthMaterial3,
    BuildingMaterial,
    NoMaterial,
    NoMaterialBuilding,
    StaticCutout,
    BaseCutout,
    LayerCutout,
    VeinCutout,
    GrowthCutout,
    GrowthCutout1,
    GrowthCutout2,
    GrowthCutout3,
    BuildingMaterialCutout,
    NoMaterialCutout,
    NoMaterialBuildingCutout,
    StaticTransparent,
    BaseTransparent,
    LayerTransparent,
    VeinTransparent,
    GrowthTransparent,
    GrowthTransparent1,
    GrowthTransparent2,
    GrowthTransparent3,
    BuildingMaterialTransparent,
    NoMaterialTransparent,
    NoMaterialBuildingTransparent,
    Count
}

public enum RotationType
{
    None,
    AwayFromWall,
    Door,
    BuildingDirection,
    Random,
    Random90,
    TreeFlat,
    TreeRound,
    TreeRoundTall
}

public class MeshContent : IContent
{
    static OpenSimplexNoise noise;
    public Dictionary<MeshLayer, CPUMesh> MeshData { get; private set; }
    TextureStorage store;

    NormalContent _normalTexture = null;
    public NormalContent NormalTexture { get { return _normalTexture; } }

    RotationType rotationType = RotationType.None;
    public Quaternion GetRotation(MapDataStore.Tile tile)
    {
        switch (rotationType)
        {
            case RotationType.None:
                return Quaternion.identity;
            case RotationType.AwayFromWall:
                {
                    Directions wallSides = tile.WallBuildingSides;
                    Vector2 average = Vector2.zero;
                    if ((wallSides & Directions.NorthWest) == Directions.NorthWest)
                        average += new Vector2(-1, 1);
                    if ((wallSides & Directions.North) == Directions.North)
                        average += new Vector2(0, 1);
                    if ((wallSides & Directions.NorthEast) == Directions.NorthEast)
                        average += new Vector2(1, 1);

                    if ((wallSides & Directions.West) == Directions.West)
                        average += new Vector2(-1, 0);
                    if ((wallSides & Directions.East) == Directions.East)
                        average += new Vector2(1, -0);

                    if ((wallSides & Directions.SouthWest) == Directions.SouthWest)
                        average += new Vector2(-1, -1);
                    if ((wallSides & Directions.South) == Directions.South)
                        average += new Vector2(0, -1);
                    if ((wallSides & Directions.SouthEast) == Directions.SouthEast)
                        average += new Vector2(1, -1);

                    if (average.magnitude < 0.001)
                        return Quaternion.Euler(0, 0, 0);


                    float angle = Mathf.Atan2(average.x, average.y) * 180 / Mathf.PI;

                    float angle90 = Mathf.Round(angle / 90) * 90;
                    float angle45 = Mathf.Round(angle / 45) * 45;

                    if (Mathf.Abs(angle90 - angle) < 30)
                        return Quaternion.Euler(0, angle90, 0);
                    else
                        return Quaternion.Euler(0, angle45, 0);
                }
            case RotationType.Door:
                {
                    Directions wallSides = tile.WallBuildingSides;

                    if ((wallSides & (Directions.North | Directions.South | Directions.East | Directions.West)) == (Directions.North | Directions.South | Directions.East | Directions.West))
                        return Quaternion.identity;
                    if ((wallSides & (Directions.North | Directions.South)) == (Directions.North | Directions.South))
                        return Quaternion.Euler(0, 90, 0); ;

                    return Quaternion.identity;
                }
            case RotationType.BuildingDirection:
                {
                    switch (tile.buildingDirection)
                    {
                        case RemoteFortressReader.BuildingDirection.NORTH:
                            return Quaternion.Euler(0, 0, 0);
                        case RemoteFortressReader.BuildingDirection.EAST:
                            return Quaternion.Euler(0, 90, 0);
                        case RemoteFortressReader.BuildingDirection.SOUTH:
                            return Quaternion.Euler(0, 180, 0);
                        case RemoteFortressReader.BuildingDirection.WEST:
                            return Quaternion.Euler(0, -90, 0);
                        default:
                            return Quaternion.Euler(0, 0, 0);
                    }
                }
            case RotationType.Random:
                {
                    if (noise == null)
                        noise = new OpenSimplexNoise();
                    float rot = (float)noise.eval(tile.position.x, tile.position.y, tile.position.z);
                    return Quaternion.Euler(0, rot * 360, 0);
                }
            case RotationType.Random90:
                {
                    if (noise == null)
                        noise = new OpenSimplexNoise();
                    float rot = (float)noise.eval(tile.position.x, tile.position.y, tile.position.z);
                    rot = Mathf.Round(rot * 4) * 90;
                    return Quaternion.Euler(0, rot, 0);
                }
            case RotationType.TreeFlat:
                {
                    Vector2 treeDir = new Vector2(-tile.positionOnTree.x, tile.positionOnTree.y);
                    if (treeDir.sqrMagnitude < 0.001)
                        return Quaternion.identity;

                    float angle = Mathf.Atan2(treeDir.x, treeDir.y) * 180 / Mathf.PI;
                    return Quaternion.Euler(0, angle, 0);
                }
            case RotationType.TreeRound:
                {
                    Vector3 treeDir = new Vector3(-tile.positionOnTree.x, tile.positionOnTree.z, tile.positionOnTree.y);
                    if (treeDir.sqrMagnitude < 0.001)
                        return Quaternion.identity;
                    return Quaternion.LookRotation(treeDir, Vector3.up);
                }
            case RotationType.TreeRoundTall:
                {
                    Vector3 treeDir = new Vector3(-tile.positionOnTree.x, tile.positionOnTree.z / 2.0f, tile.positionOnTree.y);
                    if (treeDir.sqrMagnitude < 0.001)
                        return Quaternion.identity;
                    return Quaternion.LookRotation(treeDir, Vector3.up);
                }
            default:
                return Quaternion.identity;
        }
    }

    public bool AddTypeElement(System.Xml.Linq.XElement elemtype)
    {
        XAttribute fileAtt = elemtype.Attribute("file");
        if (fileAtt == null)
        {
            //Add error message here
            return false;
        }

        if (store != null
            && (elemtype.Attribute("normal") != null
            || elemtype.Attribute("occlusion") != null
            || elemtype.Attribute("alpha") != null
            ))
        {
            _normalTexture = new NormalContent();
            _normalTexture.ExternalStorage = store;
            if (!_normalTexture.AddTypeElement(elemtype))
                _normalTexture = null;
        }

        if (fileAtt.Value == "NONE")
        {
            //This means we don't want to actually store a mesh,
            //but still want to use the category.
            MeshData = new Dictionary<MeshLayer, CPUMesh>();
        }
        else
        {
            string filePath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), fileAtt.Value);
            filePath = Path.GetFullPath(filePath);

            //	Load the OBJ in
            var lStream = new FileStream(filePath, FileMode.Open);
            var lOBJData = OBJLoader.LoadOBJ(lStream);
            lStream.Close();
            MeshData = new Dictionary<MeshLayer, CPUMesh>();
            Mesh tempMesh = new Mesh();
            foreach(MeshLayer layer in Enum.GetValues(typeof(MeshLayer)))
            {
                MeshLayer translatedLayer;
                if (layer == MeshLayer.GrowthCutout1
                    || layer == MeshLayer.GrowthCutout2
                    || layer == MeshLayer.GrowthCutout3)
                    translatedLayer = MeshLayer.GrowthCutout;
                else if (layer == MeshLayer.GrowthMaterial1
                    || layer == MeshLayer.GrowthMaterial2
                    || layer == MeshLayer.GrowthMaterial3)
                    translatedLayer = MeshLayer.GrowthMaterial;
                else if (layer == MeshLayer.GrowthTransparent1
                    || layer == MeshLayer.GrowthTransparent2
                    || layer == MeshLayer.GrowthTransparent3)
                    translatedLayer = MeshLayer.GrowthTransparent;
                else translatedLayer = layer;
                tempMesh.LoadOBJ(lOBJData, (layer.ToString()));
                if (tempMesh == null || tempMesh.vertexCount == 0)
                    continue;
                tempMesh.name = filePath + "." + layer.ToString();
                if(translatedLayer == MeshLayer.GrowthCutout
                    || translatedLayer == MeshLayer.GrowthMaterial
                    || translatedLayer == MeshLayer.GrowthTransparent)
                {
                    for(int i = (int)translatedLayer; i < (int)translatedLayer + 4; i++)
                    {
                        //This is because the tree growths can be in any order
                        //So we just copy the un-numbered one onto the rest.
                        MeshData[(MeshLayer)i] = new CPUMesh(tempMesh);
                    }
                }
                else MeshData[layer] = new CPUMesh(tempMesh);
                tempMesh.Clear();
            }
            lStream = null;
            lOBJData = null;
        }

        XAttribute rotAtt = elemtype.Attribute("rotation");
        if (rotAtt == null)
            rotationType = RotationType.None;
        else
        {
            try
            {
                rotationType = (RotationType)Enum.Parse(typeof(RotationType), rotAtt.Value);
            }
            catch
            {
                rotationType = RotationType.None;
                Debug.Log("Unknown rotation value: " + rotAtt.Value);
            }
        }
        return true;
    }


    public object ExternalStorage
    {
        set
        {
            store = value as TextureStorage;
        }
    }
}
