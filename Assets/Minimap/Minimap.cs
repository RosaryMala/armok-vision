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
    private FogMode fogMode;
    private float fogStartDistance;
    private float fogEndDistance;

    private float shadowDistance;

    private int drawRangeDown;

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
        if (GameSettings.Instance.rendering.drawRangeDown != drawRangeDown)
            SetDrawDistance();
    }

    //private void OnPreRender()
    //{
    //    shadowDistance = QualitySettings.shadowDistance;
    //    QualitySettings.shadowDistance = cam.farClipPlane;
    //    ApplyFog();
    //}

    //private void OnPostRender()
    //{
    //    QualitySettings.shadowDistance = shadowDistance;
    //    RestoreFog();
    //}

    private void ApplyFog()
    {
        //fogMode = RenderSettings.fogMode;
        fogStartDistance = RenderSettings.fogStartDistance;
        fogEndDistance = RenderSettings.fogEndDistance;

        //RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = cam.nearClipPlane + (GameMap.tileHeight * 3);
        RenderSettings.fogEndDistance = cam.farClipPlane;
    }

    private void RestoreFog()
    {
        //RenderSettings.fogMode = fogMode;
        RenderSettings.fogStartDistance = fogStartDistance;
        RenderSettings.fogEndDistance = fogEndDistance;
    }

    private void PositionCamera()
    {
        Vector3 verticalPos = GameMap.DFtoUnityCoord(GameMap.Instance.PosXTile, GameMap.Instance.PosYTile, GameMap.Instance.PosZ);
        transform.position = new Vector3(transform.position.x, verticalPos.y + cam.nearClipPlane, transform.position.z);
    }

    private void InitMap()
    {
        mapSize = DFConnection.Instance.EmbarkMapSize;
        if (texture != null)
            texture.Release();
        texture = new RenderTexture(mapSize.x * 16 * multiSample, mapSize.y * 16 * multiSample, 24);
        texture.useMipMap = true;
        texture.autoGenerateMips = true;
        rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapSize.x * 16);
        rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapSize.y * 16);
        rawImage.texture = texture;
        cam.targetTexture = texture;
        float nearPlane = (mapSize.y * 8 * GameMap.tileWidth) / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        cam.farClipPlane = nearPlane + ((GameSettings.Instance.rendering.drawRangeDown + 1) * GameMap.tileHeight);
        cam.nearClipPlane = nearPlane;
    }

    private void SetDrawDistance()
    {
        drawRangeDown = GameSettings.Instance.rendering.drawRangeDown;
        transform.position = GameMap.DFtoUnityCoord(mapSize.x * 8, mapSize.y * 8, mapSize.z) + new Vector3(-1, 0, 1);
        cam.farClipPlane = cam.nearClipPlane + ((GameSettings.Instance.rendering.drawRangeDown + 1) * GameMap.tileHeight);
    }
}
