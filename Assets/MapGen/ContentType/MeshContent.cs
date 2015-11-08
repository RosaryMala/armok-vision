using System.Xml.Linq;
using UnityEngine;
using System.IO;
using System;
using UnityExtension;

public enum MeshLayer
{
    StaticMaterial,
    BaseMaterial,
    LayerMaterial,
    VeinMaterial,
    BuildingMaterial,
    NoMaterial,
    NoMaterialBuilding,
    StaticCutout,
    BaseCutout,
    LayerCutout,
    VeinCutout,
    Growth0Cutout,
    Growth1Cutout,
    Growth2Cutout,
    Growth3Cutout,
    BuildingMaterialCutout,
    NoMaterialCutout,
    NoMaterialBuildingCutout,
    StaticTransparent,
    BaseTransparent,
    LayerTransparent,
    VeinTransparent,
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
    BuildingDirection
}

public class MeshContent : IContent
{

    public MeshData[] MeshData { get; private set; }
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
            default:
                break;
        }
        return Quaternion.identity;
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
            MeshData = new MeshData[(int)MeshLayer.Count];
        }
        else
        {
            string filePath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), fileAtt.Value);
            filePath = Path.GetFullPath(filePath);

            //	Load the OBJ in
            var lStream = new FileStream(filePath, FileMode.Open);
            var lOBJData = OBJLoader.LoadOBJ(lStream);
            lStream.Close();
            MeshData = new MeshData[(int)MeshLayer.Count];
            Mesh tempMesh = new Mesh();
            for (int i = 0; i < MeshData.Length; i++)
            {
                tempMesh.LoadOBJ(lOBJData, ((MeshLayer)i).ToString());
                MeshData[i] = new MeshData(tempMesh);
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
