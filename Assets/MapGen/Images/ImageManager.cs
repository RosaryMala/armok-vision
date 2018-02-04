using RemoteFortressReader;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }
    const int indexWidth = 16;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

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

    int coordToIndex(int x, int y)
    {
        return y * indexWidth + x;
    }

    Dictionary<MatPairStruct, Texture2D> generatedImages = new Dictionary<MatPairStruct, Texture2D>();
    Color32[] colors = new Color32[indexWidth * indexWidth];

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

        texture = new Texture2D(16, 16, TextureFormat.ARGB32, false,true);
        texture.SetPixels32(colors);
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
                    Color32 color = new Color32(
                        (byte)tile,
                        (byte)Mathf.RoundToInt(1 / itemCopy.size.x),
                        (byte)Mathf.RoundToInt(itemCopy.position.x * 255),
                        (byte)Mathf.RoundToInt(itemCopy.position.y * 255));
                    colors[coordToIndex(x, y)] = color;
                }
        }
    }
}
