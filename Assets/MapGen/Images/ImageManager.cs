using RemoteFortressReader;
using System.Collections.Generic;
using UnityEngine;

public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }

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
        return y * 16 + x;
    }

    Dictionary<MatPairStruct, Texture2D> generatedImages = new Dictionary<MatPairStruct, Texture2D>();

    public Texture2D CreateImage(ArtImage artImage)
    {
        MatPairStruct id = artImage.id;

        if (generatedImages.ContainsKey(id))
            return generatedImages[id];

        Texture2D texture = null;
        Color32[] colors = new Color32[256];
        int elementCount = artImage.elements.Count;

        if (elementCount >= 1)
        {
            var element = artImage.elements[0];
            var images = GetPattern(element.count);
            var tile = GetElementTile(element);
            foreach (var item in images)
            {
                var rect = item;
                for (int x = Mathf.RoundToInt(rect.xMin * 16); x < Mathf.RoundToInt(rect.xMax * 16); x++)
                    for (int y = Mathf.RoundToInt(rect.yMin * 16); y < Mathf.RoundToInt(rect.yMax * 16); y++)
                    {
                        Color32 color = new Color32((byte)tile, (byte)(1 / rect.size.x), (byte)(rect.position.x * 255), (byte)(rect.position.y * 255));
                        colors[coordToIndex(x, y)] = color;
                    }
            }
        }
        texture = new Texture2D(16, 16, TextureFormat.ARGB32, false,true);
        texture.SetPixels32(colors);
        texture.Apply();
        texture.name = id.ToString();
        texture.filterMode = FilterMode.Point;
        generatedImages[id] = texture;
        return texture;
    }
}
