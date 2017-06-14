using System;
using DFHack;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public static Minimap Instance { get; private set; }

    public RawImage rawImage;

    public RenderTexture texture;

    DFCoord mapSize;

    public int multiSample = 8;

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Instance = this;
    }

    private void OnPreCull()
    {
        if (DFConnection.Instance.EmbarkMapSize.x == 0)
            return;
        if (DFConnection.Instance.EmbarkMapSize != mapSize)
            InitMap();
        PositionCamera();
    }

    private void PositionCamera()
    {
        Vector3 verticalPos = GameMap.DFtoUnityCoord(GameMap.Instance.PosXTile, GameMap.Instance.PosYTile, GameMap.Instance.PosZ);
        transform.position = new Vector3(transform.position.x, verticalPos.y + 3, transform.position.z);
    }

    private void InitMap()
    {
        mapSize = DFConnection.Instance.EmbarkMapSize;
        if (texture != null)
            texture.Release();
        texture = new RenderTexture(mapSize.x * 16 * multiSample, mapSize.y * 16 * multiSample, 24);
        texture.autoGenerateMips = true;
        rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapSize.x * 16);
        rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapSize.y * 16);
        rawImage.texture = texture;
        cam.targetTexture = texture;
        transform.position = GameMap.DFtoUnityCoord(mapSize.x * 8, mapSize.y * 8, mapSize.z) + new Vector3(1,0,1);
        cam.orthographicSize = mapSize.y * 8 * GameMap.tileWidth;
    }
}
