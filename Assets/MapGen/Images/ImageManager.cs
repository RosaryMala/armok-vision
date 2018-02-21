using RemoteFortressReader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }
    const int indexWidth = 16;

    public Material engravingMaterial;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        ContentLoader.RegisterLoadCallback(LoadImages);
    }

    Dictionary<DFHack.DFCoord, MeshRenderer> engravingStore = new Dictionary<DFHack.DFCoord, MeshRenderer>();

    private void Update()
    {
        List<Engraving> engravingList;
        while ((engravingList = DFConnection.Instance.PopEngravingUpdate()) != null)
        {
            foreach (var engraving in engravingList)
            {
                if (engravingStore.ContainsKey(engraving.pos))
                    continue;

                var engravingObject = new GameObject("Engraving " + ((DFHack.DFCoord)engraving.pos));
                var renderer = engravingObject.AddComponent<MeshRenderer>();
                var filter = engravingObject.AddComponent<MeshFilter>();
                engravingObject.transform.parent = transform;
                renderer.sharedMaterial = engravingMaterial;

                if (engraving.floor)
                    filter.mesh = CreateMesh(engraving.image, Direction.Floor, 2);
                else if (engraving.north)
                    filter.mesh = CreateMesh(engraving.image, Direction.WallNorth, 2);
                else if (engraving.northwest)
                    filter.mesh = CreateMesh(engraving.image, Direction.WallNorthWest, 2);
                else if (engraving.west)
                    filter.mesh = CreateMesh(engraving.image, Direction.WallWest, 2);
                else if (engraving.southwest)
                    filter.mesh = CreateMesh(engraving.image, Direction.WallSouthWest, 2);
                else if (engraving.south)
                    filter.mesh = CreateMesh(engraving.image, Direction.WallSouth, 2);
                else if (engraving.southeast)
                    filter.mesh = CreateMesh(engraving.image, Direction.WallSouthEast, 2);
                else if (engraving.east)
                    filter.mesh = CreateMesh(engraving.image, Direction.WallEast, 2);
                else if (engraving.northeast)
                    filter.mesh = CreateMesh(engraving.image, Direction.WallNorthEast, 2);
                else
                    continue;
                engravingObject.transform.position = GameMap.DFtoUnityCoord(engraving.pos) + new Vector3(0, GameMap.floorHeight);
                var eng = engravingObject.AddComponent<CarvedEngraving>();
                eng.image = engraving;
            }
        }
    }
    #region pattern
    int GetElementTile(ArtImageElement element)
    {
        switch (element.type)
        {
            case ArtImageElementType.IMAGE_CREATURE:
                if (CreatureSpriteMap.ContainsKey(element.creature_item.mat_type))
                    return CreatureSpriteMap[element.creature_item.mat_type];
                else
                    return DFConnection.Instance.CreatureRaws[element.creature_item.mat_type].creature_tile;
            case ArtImageElementType.IMAGE_PLANT:
                if (PlantSpriteMap.ContainsKey(element.id))
                    return PlantSpriteMap[element.id];
                else
                    return DFConnection.Instance.NetPlantRawList.plant_raws[element.id].tile;
            case ArtImageElementType.IMAGE_TREE:
                return DFConnection.Instance.NetPlantRawList.plant_raws[element.id].tile;
            case ArtImageElementType.IMAGE_SHAPE:
                return DFConnection.Instance.NetLanguageList.shapes[element.id].tile;
            case ArtImageElementType.IMAGE_ITEM:
                return GetItemTile(element.creature_item);
            default:
                return 7;
        }
    }

    public int GetItemTile(MatPairStruct item)
    {
        if (ItemSpriteMap.ContainsKey(item))
            return ItemSpriteMap[item];
        item = new MatPairStruct(item.mat_type, -1);
        if (ItemSpriteMap.ContainsKey(item))
            return ItemSpriteMap[item];
        return 7;
    }

    Rect[] GetPattern(int count)
    {
        switch (count)
        {
            case 1:
                return new Rect[] { new Rect(0, 0, 1, 1) };
            case 2:
                return new Rect[] { new Rect(0.0f, 0.0f, 0.5f, 0.5f),
                                                                      new Rect(0.5f, 0.5f, 0.5f, 0.5f)};
            case 3:
                return new Rect[] { new Rect(0.0f, 0.0f, 0.5f, 0.5f), new Rect(0.5f, 0.0f, 0.5f, 0.5f),
                                                                      new Rect(0.5f, 0.5f, 0.5f, 0.5f)};
            case 4:
                return new Rect[] { new Rect(0.0f, 0.0f, 0.5f, 0.5f), new Rect(0.5f, 0.0f, 0.5f, 0.5f),
                                    new Rect(0.0f, 0.5f, 0.5f, 0.5f), new Rect(0.5f, 0.5f, 0.5f, 0.5f)};
            case 5:
                return new Rect[] { new Rect(0.0f, 0.0f, 1/3f, 1/3f),                                   new Rect(2/3f, 0.0f, 1/3f, 1/3f),
                                                                      new Rect(1/3f, 1/3f, 1/3f, 1/3f),
                                    new Rect(0.0f, 2/3f, 1/3f, 1/3f),                                   new Rect(2/3f, 2/3f, 1/3f, 1/3f)};
            case 6:
                return new Rect[] { new Rect(0.0f, 0.0f, 1/3f, 1/3f),                                   new Rect(2/3f, 0.0f, 1/3f, 1/3f),
                                    new Rect(0.0f, 1/3f, 1/3f, 1/3f),                                   new Rect(2/3f, 1/3f, 1/3f, 1/3f),
                                    new Rect(0.0f, 2/3f, 1/3f, 1/3f),                                   new Rect(2/3f, 2/3f, 1/3f, 1/3f)};
            case 7:
                return new Rect[] { new Rect(0.0f, 0.0f, 1/3f, 1/3f),                                   new Rect(2/3f, 0.0f, 1/3f, 1/3f),
                                    new Rect(0.0f, 1/3f, 1/3f, 1/3f), new Rect(1/3f, 1/3f, 1/3f, 1/3f), new Rect(2/3f, 1/3f, 1/3f, 1/3f),
                                    new Rect(0.0f, 2/3f, 1/3f, 1/3f),                                   new Rect(2/3f, 2/3f, 1/3f, 1/3f)};
            case 8:
                return new Rect[] { new Rect(0.0f, 0.0f, 1/3f, 1/3f), new Rect(1/3f, 0.0f, 1/3f, 1/3f), new Rect(2/3f, 0.0f, 1/3f, 1/3f),
                                    new Rect(0.0f, 1/3f, 1/3f, 1/3f),                                   new Rect(2/3f, 1/3f, 1/3f, 1/3f),
                                    new Rect(0.0f, 2/3f, 1/3f, 1/3f), new Rect(1/3f, 2/3f, 1/3f, 1/3f), new Rect(2/3f, 2/3f, 1/3f, 1/3f)};
            case 9:
            default:
                return new Rect[] { new Rect(0.0f, 0.0f, 1/3f, 1/3f), new Rect(1/3f, 0.0f, 1/3f, 1/3f), new Rect(2/3f, 0.0f, 1/3f, 1/3f),
                                    new Rect(0.0f, 1/3f, 1/3f, 1/3f), new Rect(1/3f, 1/3f, 1/3f, 1/3f), new Rect(2/3f, 1/3f, 1/3f, 1/3f),
                                    new Rect(0.0f, 2/3f, 1/3f, 1/3f), new Rect(1/3f, 2/3f, 1/3f, 1/3f), new Rect(2/3f, 2/3f, 1/3f, 1/3f)};

        }
    }

    int CoordToIndex(int x, int y)
    {
        return y * indexWidth + x;
    }

    Dictionary<MatPairStruct, Texture2D> generatedImages = new Dictionary<MatPairStruct, Texture2D>();
    Dictionary<MatPairStruct, Mesh> generatedMeshes = new Dictionary<MatPairStruct, Mesh>();

    Color[] colors = new Color[indexWidth * indexWidth];

    public Texture2D CreateImage(ArtImage artImage)
    {
        for(int i = 0; i < indexWidth * indexWidth; i++)
        {
            colors[i] = new Color32(0, 1, 0, 0);
        }
        MatPairStruct id = artImage.id;

        if (generatedImages.ContainsKey(id))
            return generatedImages[id];

        Texture2D texture = null;
        Rect[][] imagePattern = GetFullPattern(artImage);
        for (int i = 0; i < imagePattern.Length; i++)
        {
            foreach (var item in imagePattern[i])
            {
                CopyElement(item, GetElementTile(artImage.elements[i]));
            }
        }

        texture = new Texture2D(16, 16, TextureFormat.RGBAHalf, false,true);
        texture.SetPixels(colors);
        texture.Apply();
        texture.name = id.ToString();
        texture.filterMode = FilterMode.Point;
        generatedImages[id] = texture;
        return texture;
    }

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Vector2> indices = new List<Vector2>();

    public enum Direction
    {
        Floor,
        Wall4,
        Front,
        WallNorth,
        WallNorthWest,
        WallWest,
        WallSouthWest,
        WallSouth,
        WallSouthEast,
        WallEast,
        WallNorthEast
    }

    public Mesh CreateMesh(ArtImage artImage, Direction direction = Direction.Floor, float size = 1)
    {
        MatPairStruct id = artImage.id;

        if (generatedMeshes.ContainsKey(id))
            return generatedMeshes[id];

        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        indices.Clear();

        Rect[][] imagePattern = GetFullPattern(artImage);
        for (int i = 0; i < imagePattern.Length; i++)
        {
            int tile = GetElementTile(artImage.elements[i]);
            foreach (var item in imagePattern[i])
            {
                switch (direction)
                {
                    case Direction.Floor:
                        AddRectToMesh(item, tile, Matrix4x4.Scale(new Vector3(size, size, size)));
                        break;
                    case Direction.Wall4:
                        //South
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(0, GameMap.tileWidth / 2, -GameMap.tileWidth / 2),
                            Quaternion.Euler(-90, 0, 0),
                            new Vector3(size, size, size)));
                        //West
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(-GameMap.tileWidth / 2, GameMap.tileWidth / 2, 0),
                            Quaternion.Euler(-90, 0, 90),
                            new Vector3(size, size, size)));
                        //North
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(0, GameMap.tileWidth / 2, GameMap.tileWidth / 2),
                            Quaternion.Euler(-90, 0, 180),
                            new Vector3(size, size, size)));
                        //East
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(GameMap.tileWidth / 2, GameMap.tileWidth / 2, 0),
                            Quaternion.Euler(-90, 0, 270),
                            new Vector3(size, size, size)));
                        break;
                    case Direction.Front:
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            Vector3.zero,
                            Quaternion.Euler(-90, 0, 0),
                            new Vector3(size, size, size)));
                        break;
                    case Direction.WallNorth:
                        //North
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(0, GameMap.tileWidth / 2, GameMap.tileWidth / 2),
                            Quaternion.Euler(-90, 0, 180),
                            new Vector3(size, size, size)));
                        break;
                    case Direction.WallNorthWest:
                        //West
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(-GameMap.tileWidth / 2, GameMap.tileWidth / 2, 0),
                            Quaternion.Euler(-90, 0, 90),
                            new Vector3(size, size, size)));
                        //North
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(0, GameMap.tileWidth / 2, GameMap.tileWidth / 2),
                            Quaternion.Euler(-90, 0, 180),
                            new Vector3(size, size, size)));
                        break;
                    case Direction.WallWest:
                        //West
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(-GameMap.tileWidth / 2, GameMap.tileWidth / 2, 0),
                            Quaternion.Euler(-90, 0, 90),
                            new Vector3(size, size, size)));
                        break;
                    case Direction.WallSouthWest:
                        //South
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(0, GameMap.tileWidth / 2, -GameMap.tileWidth / 2),
                            Quaternion.Euler(-90, 0, 0),
                            new Vector3(size, size, size)));
                        //West
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(-GameMap.tileWidth / 2, GameMap.tileWidth / 2, 0),
                            Quaternion.Euler(-90, 0, 90),
                            new Vector3(size, size, size)));
                        break;
                    case Direction.WallSouth:
                        //South
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(0, GameMap.tileWidth / 2, -GameMap.tileWidth / 2),
                            Quaternion.Euler(-90, 0, 0),
                            new Vector3(size, size, size)));
                        break;
                    case Direction.WallSouthEast:
                        //South
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(0, GameMap.tileWidth / 2, -GameMap.tileWidth / 2),
                            Quaternion.Euler(-90, 0, 0),
                            new Vector3(size, size, size)));
                        //East
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(GameMap.tileWidth / 2, GameMap.tileWidth / 2, 0),
                            Quaternion.Euler(-90, 0, 270),
                            new Vector3(size, size, size)));
                        break;
                    case Direction.WallEast:
                        //East
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(GameMap.tileWidth / 2, GameMap.tileWidth / 2, 0),
                            Quaternion.Euler(-90, 0, 270),
                            new Vector3(size, size, size)));
                        break;
                    case Direction.WallNorthEast:
                        //North
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(0, GameMap.tileWidth / 2, GameMap.tileWidth / 2),
                            Quaternion.Euler(-90, 0, 180),
                            new Vector3(size, size, size)));
                        //East
                        AddRectToMesh(item, tile, Matrix4x4.TRS(
                            new Vector3(GameMap.tileWidth / 2, GameMap.tileWidth / 2, 0),
                            Quaternion.Euler(-90, 0, 270),
                            new Vector3(size, size, size)));
                        break;
                    default:
                        break;
                }
            }
        }
        var mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetUVs(3, indices);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        generatedMeshes[id] = mesh;
        return mesh;
    }

    private void AddRectToMesh(Rect item, int tile, Matrix4x4 matrix)
    {
        int start = vertices.Count;
        vertices.Add(matrix.MultiplyPoint3x4(new Vector3(item.xMin - 0.5f, 0, item.yMin - 0.5f)));
        vertices.Add(matrix.MultiplyPoint3x4(new Vector3(item.xMax - 0.5f, 0, item.yMin - 0.5f)));
        vertices.Add(matrix.MultiplyPoint3x4(new Vector3(item.xMin - 0.5f, 0, item.yMax - 0.5f)));
        vertices.Add(matrix.MultiplyPoint3x4(new Vector3(item.xMax - 0.5f, 0, item.yMax - 0.5f)));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        indices.Add(new Vector2(tile, 0));
        indices.Add(new Vector2(tile, 0));
        indices.Add(new Vector2(tile, 0));
        indices.Add(new Vector2(tile, 0));
        triangles.Add(start + 0);
        triangles.Add(start + 2);
        triangles.Add(start + 1);

        triangles.Add(start + 2);
        triangles.Add(start + 3);
        triangles.Add(start + 1);
    }

    private Rect[][] GetFullPattern(ArtImage artImage)
    {
        //Todo: More advanced combinations, depending on verbs.
        if(artImage.elements.Count == 2)
        {
            //There's two elements, so we can easily do surrounds.
            int surroundIndex = artImage.properties.FindIndex(x => x.type == ArtImagePropertyType.TRANSITIVE_VERB && x.verb == ArtImageVerb.VERB_SURROUNDEDBY);
            if(surroundIndex >= 0)
            {
                var property = artImage.properties[surroundIndex];
                var surrounded = new Rect[2][];
                surrounded[property.subject] = new Rect[] { new Rect(0.25f, 0.25f, 0.5f, 0.5f) };
                surrounded[property.@object] = new Rect[] {
                    new Rect(0.0f, 0.0f, 0.25f, 0.25f), new Rect(0.25f, 0.0f, 0.25f, 0.25f), new Rect(0.5f, 0.0f, 0.25f, 0.25f), new Rect(0.75f, 0.0f, 0.25f, 0.25f),
                    new Rect(0.0f, 0.25f, 0.25f, 0.25f),                                                                          new Rect(0.75f, 0.25f, 0.25f, 0.25f),
                    new Rect(0.0f, 0.5f, 0.25f, 0.25f),                                                                          new Rect(0.75f, 0.5f, 0.25f, 0.25f),
                    new Rect(0.0f, 0.75f, 0.25f, 0.25f), new Rect(0.25f, 0.75f, 0.25f, 0.25f), new Rect(0.5f, 0.75f, 0.25f, 0.25f), new Rect(0.75f, 0.75f, 0.25f, 0.25f),
                };
                return surrounded;
            }
        }


        var mainPattern = GetPattern(artImage.elements.Count);
        var outPut = new Rect[mainPattern.Length][];
        //We use the painpattern length because it may have less than the full amount of elements, if the pattern is very large.
        for (int i = 0; i < mainPattern.Length; i++)
        {
            var element = artImage.elements[i];
            outPut[i] = GetPattern(element.count);
            for(int j = 0; j < outPut[i].Length;j++)
            {
                var min = outPut[i][j].min;
                var max = outPut[i][j].max;
                min.x *= mainPattern[i].size.x;
                min.y *= mainPattern[i].size.y;
                max.x *= mainPattern[i].size.x;
                max.y *= mainPattern[i].size.y;
                outPut[i][j].min = min;
                outPut[i][j].max = max;
                outPut[i][j].position += mainPattern[i].position;
            }
        }
        return outPut;
    }

    private void CopyElement(Rect rect, int tile)
    {
        for (int x = Mathf.RoundToInt(rect.xMin * indexWidth); x < Mathf.RoundToInt(rect.xMax * indexWidth); x++)
            for (int y = Mathf.RoundToInt(rect.yMin * indexWidth); y < Mathf.RoundToInt(rect.yMax * indexWidth); y++)
            {
                Color color = new Color(
                    tile,
                    Mathf.RoundToInt(1 / rect.size.x),
                    Mathf.RoundToInt(rect.position.x),
                    Mathf.RoundToInt(rect.position.y));
                colors[CoordToIndex(x, y)] = color;
            }
    }
    #endregion

    #region imageList

    public Texture2D dfSpriteMap;

    public Texture2DArray imageSpriteArray;
    public Texture2DArray imageSpriteNormals;

    Dictionary<MatPairStruct, int> ItemSpriteMap = new Dictionary<MatPairStruct, int>();

    Dictionary<int, int> CreatureSpriteMap = new Dictionary<int, int>();
    Dictionary<int, int> PlantSpriteMap = new Dictionary<int, int>();

    IEnumerator LoadImages()
    {
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();

        List<Texture2D> textureList = new List<Texture2D>();

        //DFTiles:
        {
            int sourceWidth = dfSpriteMap.width / 16;
            int sourceHeight = dfSpriteMap.height / 16;
            for (int y = 16 - 1; y >= 0; y--)
                for (int x = 0; x < 16; x++)
                {
                    var pixels = dfSpriteMap.GetPixels(sourceWidth * x, sourceHeight * y, sourceWidth, sourceHeight);
                    var tempTex = new Texture2D(sourceWidth, sourceHeight, TextureFormat.ARGB32, false);
                    tempTex.SetPixels(pixels);
                    tempTex.Apply();
                    if (tempTex.width != 32 || tempTex.height != 32)
                        TextureScale.Bilinear(tempTex, 32, 32);
                    textureList.Add(tempTex);
                }
        }
        //IMAGE_CREATURE:
        foreach (var creature in DFConnection.Instance.CreatureRaws)
        {
            string token = creature.creature_id;
            Texture2D sprite = Resources.Load<Texture2D>("Images/Creatures/" + token);
            if (sprite == null)
            {
                //Try again without stupid numbers
                token.TrimEnd('1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_');
                sprite = Resources.Load<Texture2D>("Images/Creatures/" + token);
            }
            if (sprite == null)
            {
                Debug.LogWarning("Could not find art image for " + token);
                continue;
            }
            if (sprite.width != 32 || sprite.height != 32)
                TextureScale.Bilinear(sprite, 32, 32);
            CreatureSpriteMap[creature.index] = textureList.Count;
            textureList.Add(sprite);

            if (stopWatch.ElapsedMilliseconds > 100)
            {
                yield return null;
                stopWatch.Reset();
                stopWatch.Start();
            }
        }
        //IMAGE_PLANT:
        //IMAGE_TREE:
        //IMAGE_SHAPE:
        //IMAGE_ITEM:
        foreach (var item in DFConnection.Instance.NetItemList.material_list)
        {
            string token = item.id;
            Texture2D sprite = Resources.Load<Texture2D>("Images/Items/" + token);
            if (sprite == null)
            {
                //Try again without stupid numbers
                token.TrimEnd('1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_');
                sprite = Resources.Load<Texture2D>("Images/Items/" + token);
            }
            if (sprite == null)
            {
                Debug.LogWarning("Could not find art image for " + token);
                continue;
            }

            if(sprite.width != 32 || sprite.height != 32)
                TextureScale.Bilinear(sprite, 32, 32);

            ItemSpriteMap[item.mat_pair] = textureList.Count;
            if (item.mat_pair.mat_type == 53) //Plant
                PlantSpriteMap[item.mat_pair.mat_index] = textureList.Count;
            textureList.Add(sprite);

            if (stopWatch.ElapsedMilliseconds > 100)
            {
                yield return null;
                stopWatch.Reset();
                stopWatch.Start();
            }
        }

        imageSpriteArray = new Texture2DArray(32, 32, textureList.Count, TextureFormat.ARGB32, false);
        imageSpriteNormals = new Texture2DArray(32, 32, textureList.Count, TextureFormat.ARGB32, false);
        for (int i = 0; i < textureList.Count; i++)
        {
            var pixels = textureList[i].GetPixels();
            imageSpriteArray.SetPixels(pixels, i);
            imageSpriteNormals.SetPixels(TextureTools.Bevel(pixels, 32,32), i);
        }
        imageSpriteArray.Apply();
        imageSpriteNormals.Apply();
        Shader.SetGlobalTexture("_ImageAtlas", imageSpriteArray);
        Shader.SetGlobalTexture("_ImageBumpAtlas", imageSpriteNormals);
    }

    #endregion
}
