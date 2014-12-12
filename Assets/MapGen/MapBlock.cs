using DFHack;
using System.Collections.Generic;
using UnityEngine;
using RemoteFortressReader;

public class MapBlock : MonoBehaviour
{
    public const int blockWidthTiles = 16;
    public const int blockAreaTiles = blockWidthTiles * blockWidthTiles;
    public static float tileHeight = 3.0f;
    public static float tileWidth = 2.0f;
    public static float blockWidth
    {
        get { return tileWidth * 16; }
    }
    public static float BlockHeight
    {
        get { return tileHeight; }
    }
    public static float floorHeight = 0.5f;
    public static float rampDistance = 2.0f*tileWidth;
    public string coordString
    {
        get { return coordinates.x + "," + coordinates.y + "," + coordinates.z; }
    }
    DFCoord coordinates;
    DFCoord2d map_coords;
    GameMap parent;

    [SerializeField]
    TiletypeShape[] terrain = new TiletypeShape[blockAreaTiles];
    [SerializeField]
    Color32[] colors = new Color32[blockAreaTiles];

    List<Vector3> finalVertices = new List<Vector3>();
    List<int> finalFaces = new List<int>();
    List<Color32> finalVertexColors = new List<Color32>();
    List<Vector2> finalUVs = new List<Vector2>();

    public enum Openness
    {
        air,
        mixed,
        stone
    }
    Openness openness;

    public void SetOpenness()
    {
        int air = 0;
        int solid = 0;
        for (int x = 0; x < blockWidthTiles; x++)
            for (int y = 0; y < blockWidthTiles; y++)
            {
                if (terrain[y * blockWidthTiles + x] == TiletypeShape.EMPTY)
                    air++;
                else if (terrain[y * blockWidthTiles + x] == TiletypeShape.WALL)
                    solid++;
            }
        if (air == blockAreaTiles)
            openness = Openness.air;
        else if (solid == blockAreaTiles)
            openness = Openness.stone;
        else openness = Openness.mixed;
    }

    public void Reposition(RemoteFortressReader.BlockList input)
    {
        int diff_x = input.map_x - map_coords.x;
        int diff_y = input.map_y - map_coords.y;
        coordinates.x += (diff_x * 3);
        coordinates.y += (diff_y * 3);
        map_coords.x = input.map_x;
        map_coords.y = input.map_y;
        SetUnityPosition();
    }

    public void SetUnityPosition()
    {
        transform.position = new Vector3(coordinates.x * tileWidth, coordinates.z * tileHeight, coordinates.y * (-tileWidth));
    }

    public Openness GetOpenness()
    {
        return openness;
    }

    public Color32 GetColor(DFCoord2d position)
    {
        if ((position.x + position.y * blockWidthTiles) < colors.Length)
            return colors[position.x + position.y * blockWidthTiles];
        else return Color.white;
    }

    public void SetColor(DFCoord2d position, Color32 input)
    {
        if ((position.x + position.y * blockWidthTiles) < colors.Length)
            colors[position.x + position.y * blockWidthTiles] = input;
    }

    public void SetSingleTile(DFCoord2d position, TiletypeShape tile)
    {
        terrain[position.x + position.y * blockWidthTiles] = tile;
        SetOpenness();
    }
    public void SetAllTiles(TiletypeShape tile)
    {
        for(int i = 0; i < terrain.GetLength(0);i++)
        {
            terrain[i] = tile;
        }
        SetOpenness();
    }

    public void SetAllTiles(RemoteFortressReader.MapBlock DFBlock, RemoteFortressReader.BlockList blockList, RemoteFortressReader.TiletypeList tiletypeList)
    {
        if (DFBlock.tiles.Count != terrain.Length)
        {
            Debug.LogError("Map Block has " + DFBlock.tiles.Count + " tiles, should be " + terrain.Length);
        }
        for (int i = 0; i < DFBlock.tiles.Count; i++)
        {
            terrain[i] = tiletypeList.tiletype_list[DFBlock.tiles[i]].shape;
            colors[i] = Color.white;
        }
        SetOpenness();
        coordinates.x = DFBlock.map_x;
        coordinates.y = DFBlock.map_y;
        coordinates.z = DFBlock.map_z;
        map_coords.x = blockList.map_x;
        map_coords.y = blockList.map_y;
        SetUnityPosition();
    }

    public TiletypeShape GetSingleTile(DFCoord2d position)
    {
        if (position.x >= 0 && position.x < blockWidthTiles && position.y >= 0 && position.y < blockWidthTiles)
            return terrain[position.x + position.y * blockWidthTiles];
        else
            return TiletypeShape.EMPTY;
    }

    public void Regenerate()
    {
        finalVertices.Clear();
        finalFaces.Clear();
        finalVertexColors.Clear();
        finalUVs.Clear();

        if (openness == Openness.air)
        {
        }
        else
        {
            for (int i = 0; i < blockWidthTiles; i++)
                for (int j = 0; j < blockWidthTiles; j++)
                {
                    DFCoord2d here = new DFCoord2d(i, j);
                    switch (GetSingleTile(here))
                    {
                        case TiletypeShape.WALL:
                            AddTopFace(here, tileHeight);
                            break;
                        case TiletypeShape.RAMP:
                        case TiletypeShape.FLOOR:
                            AddTopFace(here, floorHeight);
                            break;
                    }
                    AddSideFace(here, FaceDirection.North);
                    AddSideFace(here, FaceDirection.South);
                    AddSideFace(here, FaceDirection.East);
                    AddSideFace(here, FaceDirection.West);
                }
        }
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mesh.vertices = finalVertices.ToArray();
        mesh.uv = finalUVs.ToArray();
        mesh.colors32 = finalVertexColors.ToArray();
        mesh.triangles = finalFaces.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    enum FaceDirection
    {
        Up,
        Down,
        North,
        South,
        East,
        West
    }

    TiletypeShape GetRelativeTile(DFCoord2d position, FaceDirection direction)
    {
        DFCoord2d relativePosition = new DFCoord2d(position.x, position.y);
        switch (direction)
        {
            case FaceDirection.North:
                relativePosition.y--;
                break;
            case FaceDirection.South:
                relativePosition.y++;
                break;
            case FaceDirection.East:
                relativePosition.x++;
                break;
            case FaceDirection.West:
                relativePosition.x--;
                break;
        }
        return GetSingleTile(relativePosition);
    }

    enum Layer
    {
        Base,
        Floor,
        Top
    }

    float convertDistanceToOffset(float input)
    {
        if (input == float.MaxValue)
            return 0;
        input = Mathf.Pow(input, 0.5f);
        input = (rampDistance - input) / rampDistance;
        if (input < 0)
            return 0;
        return Mathf.Sin(input * Mathf.PI / 4.0f) * tileHeight;
    }

    Vector3 AdjustForRamps(Vector3 input, Layer layer = Layer.Floor)
    {
        return input;
    }

    void AddSideFace(DFCoord2d position, FaceDirection direction)
    {
        Layer topLayer = Layer.Top;
        Layer bottomLayer = Layer.Base;
        float currentFloorHeight = -0.5f * tileHeight;
        float adjacentFloorHeight = -0.5f * tileHeight;
        switch (GetSingleTile(position))
        {
            case TiletypeShape.WALL:
                currentFloorHeight = 0.5f * tileHeight;
                topLayer = Layer.Top;
                break;
            case TiletypeShape.RAMP:
            case TiletypeShape.FLOOR:
                currentFloorHeight = floorHeight - (0.5f * tileHeight);
                topLayer = Layer.Floor;
                break;
            default:
                break;
        }
        switch (GetRelativeTile(position, direction))
        {
            case TiletypeShape.WALL:
                adjacentFloorHeight = 0.5f * tileHeight;
                bottomLayer = Layer.Top;
                break;
            case TiletypeShape.FLOOR:
                adjacentFloorHeight = floorHeight - (0.5f * tileHeight);
                bottomLayer = Layer.Floor;
                break;
            default:
                break;
        }
        if (currentFloorHeight <= adjacentFloorHeight)
            return;
        int startindex = finalVertices.Count;
        int uvPos = 0;
        switch (direction)
        {
            case FaceDirection.North:
                finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, currentFloorHeight, -(position.y - 0.5f) * tileWidth), topLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, currentFloorHeight, -(position.y - 0.5f) * tileWidth), topLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, adjacentFloorHeight, -(position.y - 0.5f) * tileWidth), bottomLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, adjacentFloorHeight, -(position.y - 0.5f) * tileWidth), bottomLayer));
                uvPos = position.x;
                break;
            case FaceDirection.South:
                finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, currentFloorHeight, -(position.y + 0.5f) * tileWidth), topLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, currentFloorHeight, -(position.y + 0.5f) * tileWidth), topLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, adjacentFloorHeight, -(position.y + 0.5f) * tileWidth), bottomLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, adjacentFloorHeight, -(position.y + 0.5f) * tileWidth), bottomLayer));
                uvPos = 16 - position.x;
                break;
            case FaceDirection.East:
                finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, currentFloorHeight, -(position.y + 0.5f) * tileWidth), topLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, currentFloorHeight, -(position.y - 0.5f) * tileWidth), topLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, adjacentFloorHeight, -(position.y + 0.5f) * tileWidth), bottomLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, adjacentFloorHeight, -(position.y - 0.5f) * tileWidth), bottomLayer));
                uvPos = position.y;
                break;
            case FaceDirection.West:
                finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, currentFloorHeight, -(position.y - 0.5f) * tileWidth), topLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, currentFloorHeight, -(position.y + 0.5f) * tileWidth), topLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, adjacentFloorHeight, -(position.y - 0.5f) * tileWidth), bottomLayer));
                finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, adjacentFloorHeight, -(position.y + 0.5f) * tileWidth), bottomLayer));
                uvPos = 16 - position.y;
                break;
            default:
                break;
        }
        finalUVs.Add(new Vector2(-(float)(uvPos + 1) / 16.0f, -(float)(0) / 16.0f));
        finalUVs.Add(new Vector2(-(float)(uvPos) / 16.0f, -(float)(0) / 16.0f));
        finalUVs.Add(new Vector2(-(float)(uvPos + 1) / 16.0f, -(float)(0 + 1) / 16.0f));
        finalUVs.Add(new Vector2(-(float)(uvPos) / 16.0f, -(float)(0 + 1) / 16.0f));

        finalVertexColors.Add(GetColor(position));
        finalVertexColors.Add(GetColor(position));
        finalVertexColors.Add(GetColor(position));
        finalVertexColors.Add(GetColor(position));

        finalFaces.Add(startindex);
        finalFaces.Add(startindex + 1);
        finalFaces.Add(startindex + 2);

        finalFaces.Add(startindex + 1);
        finalFaces.Add(startindex + 3);
        finalFaces.Add(startindex + 2);
    }

    void AddTopFace(DFCoord2d position, float height)
    {
        Layer layer = Layer.Base;
        if (GetSingleTile(position) == TiletypeShape.FLOOR)
            layer = Layer.Floor;
        else if (GetSingleTile(position) == TiletypeShape.WALL)
            layer = Layer.Top;
        height -= 0.5f * tileHeight;
        //Todo: Weld vertices that should be welded
        //On second though, not with vertex colors there.
        int startindex = finalVertices.Count;
        finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, height, -(position.y - 0.5f) * tileWidth), layer));
        finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, height, -(position.y - 0.5f) * tileWidth), layer));
        finalVertices.Add(AdjustForRamps(new Vector3((position.x - 0.5f) * tileWidth, height, -(position.y + 0.5f) * tileWidth), layer));
        finalVertices.Add(AdjustForRamps(new Vector3((position.x + 0.5f) * tileWidth, height, -(position.y + 0.5f) * tileWidth), layer));

        finalUVs.Add(new Vector2((float)(position.x) / 16.0f, -(float)(position.y) / 16.0f));
        finalUVs.Add(new Vector2((float)(position.x + 1) / 16.0f, -(float)(position.y) / 16.0f));
        finalUVs.Add(new Vector2((float)(position.x) / 16.0f, -(float)(position.y + 1) / 16.0f));
        finalUVs.Add(new Vector2((float)(position.x + 1) / 16.0f, -(float)(position.y + 1) / 16.0f));

        finalVertexColors.Add(GetColor(position));
        finalVertexColors.Add(GetColor(position));
        finalVertexColors.Add(GetColor(position));
        finalVertexColors.Add(GetColor(position));

        finalFaces.Add(startindex);
        finalFaces.Add(startindex + 1);
        finalFaces.Add(startindex + 2);

        finalFaces.Add(startindex + 1);
        finalFaces.Add(startindex + 3);
        finalFaces.Add(startindex + 2);
    }
}
