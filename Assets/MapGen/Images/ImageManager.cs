using RemoteFortressReader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }
    const int indexWidth = 16;

    public MeshRenderer floorEngravingPrefab;
    public MeshRenderer wallEngravingPrefab;

    int tileIndexProp;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        tileIndexProp = Shader.PropertyToID("_TileIndex");

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
                MeshRenderer placedEngraving;
                if (engraving.floor)
                    placedEngraving = Instantiate(floorEngravingPrefab, transform);
                else if (engraving.north || engraving.northwest || engraving.west || engraving.southwest
                    || engraving.south || engraving.southeast || engraving.east || engraving.northeast)
                    placedEngraving = Instantiate(wallEngravingPrefab, transform);
                else
                    continue;
                placedEngraving.transform.position = GameMap.DFtoUnityCoord(engraving.pos) + new Vector3(0, GameMap.floorHeight);
                placedEngraving.material.SetTexture(tileIndexProp, CreateImage(engraving.image));
                engravingStore[engraving.pos] = placedEngraving;
                var eng = placedEngraving.GetComponent<CarvedEngraving>();
                if (eng != null)
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
                return DFConnection.Instance.CreatureRaws[element.creature_item.mat_type].creature_tile;
            case ArtImageElementType.IMAGE_PLANT:
                return DFConnection.Instance.NetPlantRawList.plant_raws[element.id].tile;
            case ArtImageElementType.IMAGE_TREE:
                return DFConnection.Instance.NetPlantRawList.plant_raws[element.id].tile;
            case ArtImageElementType.IMAGE_SHAPE:
                return DFConnection.Instance.NetLanguageList.shapes[element.id].tile;
            case ArtImageElementType.IMAGE_ITEM:
                if (ItemSpriteMap.ContainsKey(element.creature_item))
                    return ItemSpriteMap[element.creature_item];
                else
                    return 7;
            default:
                return 7;
        }
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
        var imagePattern = GetPattern(artImage.elements.Count);
        for(int i = 0; i < artImage.elements.Count; i++)
        {
            CopyElement(imagePattern[i], artImage.elements[i]);
        }

        texture = new Texture2D(16, 16, TextureFormat.RGBAHalf, false,true);
        texture.SetPixels(colors);
        texture.Apply();
        texture.name = id.ToString();
        texture.filterMode = FilterMode.Point;
        generatedImages[id] = texture;
        return texture;
    }

    private void CopyElement(Rect rect, ArtImageElement artImageElement)
    {
        var images = GetPattern(artImageElement.count);
        var tile = GetElementTile(artImageElement);
        foreach (var item in images)
        {
            var itemCopy = item;
            itemCopy.min *= rect.size.x;
            itemCopy.max *= rect.size.y;
            itemCopy.position += rect.position;
            for (int x = Mathf.RoundToInt(itemCopy.xMin * indexWidth); x < Mathf.RoundToInt(itemCopy.xMax * indexWidth); x++)
                for (int y = Mathf.RoundToInt(itemCopy.yMin * indexWidth); y < Mathf.RoundToInt(itemCopy.yMax * indexWidth); y++)
                {
                    Color color = new Color(
                        tile,
                        Mathf.RoundToInt(1 / itemCopy.size.x),
                        Mathf.RoundToInt(itemCopy.position.x),
                        Mathf.RoundToInt(itemCopy.position.y));
                    colors[CoordToIndex(x, y)] = color;
                }
        }
    }
    #endregion

    #region imageList

    public Texture2D dfSpriteMap;

    public Texture2DArray imageSpriteArray;
    public Texture2DArray imageSpriteNormals;

    Dictionary<MatPairStruct, int> ItemSpriteMap = new Dictionary<MatPairStruct, int>();

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
                Debug.LogWarning("Could not find art image for " + token);
                continue;
            }

            if(sprite.width != 32 || sprite.height != 32)
                TextureScale.Bilinear(sprite, 32, 32);

            ItemSpriteMap[item.mat_pair] = textureList.Count;
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
