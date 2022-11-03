using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class GameSettings : MonoBehaviour
{
    public enum LandscapeDetail
    {
        Off,
        Low,
        High
    }

    public GoogleAnalyticsV4 analytics;

    [Serializable]
    public class Meshing
    {
        public int meshingThreads = 4;
        public int queueLimit = 1;
        public VoxelGenerator.CornerType cornerType = VoxelGenerator.CornerType.Rounded;
    }

    [Serializable]
    public class Rendering
    {
        public int drawRangeSide = 4;
        public int drawRangeUp = 1;
        public int drawRangeDown = 5;
        public int maxBlocksToDraw = 460800;
        public int maxTextureSize = 1024;
        public int textureAtlasSize = 4096;
        public bool debugTextureAtlas = false;
        public bool drawClouds = true;
        public LandscapeDetail distantTerrainDetail = LandscapeDetail.High;
        public int vSyncCount = 0;
        public int targetFrameRate = 30;
        public bool showHiddenTiles = false;
        public bool fog = true;
        public int maxItemsPerTile = 10;
        public float itemDrawDistance = 50;
        public float creatureDrawDistance = 50;
    }

    public static void ClampToMaxSize(Texture2D texture)
    {
        if (texture.width > Instance.rendering.maxTextureSize || texture.height > Instance.rendering.maxTextureSize)
        {
            if (texture.width > texture.height)
            {
                TextureScale.Bilinear(
                    texture,
                    Instance.rendering.maxTextureSize,
                    Instance.rendering.maxTextureSize * texture.height / texture.width);
            }
            else
            {
                TextureScale.Bilinear(
                    texture,
                    Instance.rendering.maxTextureSize * texture.width / texture.height,
                    Instance.rendering.maxTextureSize);
            }
        }
    }

    public static void MatchSizes(Texture2D a, Texture2D b)
    {
        if (a.width != b.width || a.height != b.height)
        {
            TextureScale.Bilinear(a, Mathf.Max(a.width, b.width), Mathf.Max(a.height, b.width));
            TextureScale.Bilinear(b, Mathf.Max(a.width, b.width), Mathf.Max(a.height, b.width));
        }

    }

    public static void MatchSizes(Texture2D[] textures)
    {
        int maxWidth = int.MinValue;
        int maxHeight = int.MinValue;
        foreach (var item in textures)
        {
            maxWidth = Mathf.Max(item.width, maxWidth);
            maxHeight = Mathf.Max(item.height, maxHeight);
        }
        foreach (var item in textures)
        {
            if (item.width != maxWidth || item.height != maxHeight)
            {
                TextureScale.Bilinear(item, maxWidth, maxHeight);
            }
        }
    }

    [Serializable]
    public class CameraSettings
    {
        public float fieldOfView = 70;
        public bool deferredRendering = true;
        public bool SSAO = true;
        public bool postProcessing = true;
    }

    public enum UnitDetail
    {
        None,
        ASCII,
        SDSprites,
        HDSprites,
        Models
    }

    public enum UnitScale
    {
        Fixed,
        Logarithmic,
        Real
    }

    [Serializable]
    public class Units
    {
        public UnitScale scaleUnits = UnitScale.Real;
        public float chibiness = 2f;
        public UnitDetail unitDetail = UnitDetail.Models;
        public bool censorField = true;
    }

    public enum AnalyticsChoice
    {
        Unknown,
        Yes,
        No
    }

    [Serializable]
    public class Game
    {
        public bool showDFScreen = false;
        public AnalyticsChoice analytics = AnalyticsChoice.Unknown;
        public bool askToUpdatePlugin = true;
        public string serverAddress = "localhost";
        public int serverPort = 5000;
        public bool checkForUpdates = true;
    }
    [Serializable]
    public class Debug
    {
        public bool drawDebugInfo = false;
        public bool saveBuildingList = false;
        public bool saveCreatureList = false;
        public bool saveItemList = false;
        public bool saveMaterialList = false;
        public bool savePlantList = false;
        public bool saveTiletypeList = false;
    }

    [Serializable]
    public class UpdateTimers
    {
        public float blockUpdate = 500;
    }

    [Serializable]
    public class Settings
    {
        public Meshing meshing = new Meshing();
        public Rendering rendering = new Rendering();
        public Units units = new Units();
        public CameraSettings camera = new CameraSettings();
        public Game game = new Game();
        public Debug debug = new Debug();
        public UpdateTimers updateTimers = new UpdateTimers();
    }

    static Settings _instance;

    public static Settings Instance
    {
        get
        {
            if (_instance == null)
                Init();
            return _instance;
        }
    }

    public List<Camera> mainCameras;

    public Light[] LightList;

    static void DeserializeIni(string filename)
    {
        if (!File.Exists(filename))
        {
            _instance = new Settings();
            return;
        }

        _instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(filename));
    }

    static void SerializeIni(string filename)
    { 
        File.WriteAllText(filename, JsonConvert.SerializeObject(Instance, Formatting.Indented));
    }



    static string filename;

    // This function is called when the MonoBehaviour will be destroyed
    public void OnDestroy()
    {
        Save();
    }

    void Save()
    {
        if (string.IsNullOrWhiteSpace(filename))
            SetupFileName();
        SerializeIni(filename);
    }

    static void SetupFileName()
    {
        string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.productName);
        if (!Directory.Exists(configDir))
            Directory.CreateDirectory(configDir);
        filename = Path.Combine(configDir, "Config.json");
    }

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        if (mainCameras.Count > 0)
            Instance.camera.fieldOfView = mainCameras[0].fieldOfView;
        Init();
        Application.targetFrameRate = Instance.rendering.targetFrameRate;
        QualitySettings.vSyncCount = Instance.rendering.vSyncCount;
        if (mainCameras.Count > 0)
            foreach (Camera camera in mainCameras)
            {
                camera.fieldOfView = Instance.camera.fieldOfView;
            }
        if (Instance.game.analytics == AnalyticsChoice.Yes)
            analytics.gameObject.SetActive(true);
        UpdatePostProcessing();
    }

    static void Init()
    {
        if (string.IsNullOrWhiteSpace(filename))
            SetupFileName();
        DeserializeIni(filename);
    }

    void SetShadows(bool input)
    {
        foreach (Light item in LightList)
        {
            switch (input)
            {
                case true:
                    item.shadows = LightShadows.Soft;
                    break;
                default:
                    item.shadows = LightShadows.None;
                    break;
            }
        }
    }

    void SetSlider(GameObject go, float value)
    {
        Slider slider = go.GetComponent<Slider>();
        if (slider == null) return;
        slider.value = value;
    }
    void SetSlider(Slider slider, float value)
    {
        if (slider == null) return;
        slider.value = value;
    }

#region Variable change events

#region Deferred
    Slider deferredSlider;
    public void InitDeferredRendering(GameObject go)
    {
        deferredSlider = go.GetComponent<Slider>();
        SetSlider(deferredSlider, Convert.ToInt32(Instance.camera.deferredRendering));
    }
    public void SetDeferredRendering(float value)
    {
        Instance.camera.deferredRendering = Convert.ToBoolean(value);
        UpdateDeferredRendering();
        UpdatePostProcessing();
        SetSlider(deferredSlider, Convert.ToInt32(Instance.camera.deferredRendering));
    }
    public void UpdateDeferredRendering()
    {
        foreach (Camera camera in mainCameras)
        {
                if (Instance.camera.deferredRendering)
                    camera.renderingPath = RenderingPath.DeferredShading;
                else
                    camera.renderingPath = RenderingPath.Forward;
        }
        if (postprocessSlider != null)
            postprocessSlider.gameObject.SetActive(Instance.camera.deferredRendering);
    }
#endregion

#region PostProcessing
    Slider postprocessSlider;
    public void InitPostProcessing(GameObject go)
    {
        postprocessSlider = go.GetComponent<Slider>();
        SetSlider(postprocessSlider, Convert.ToInt32(Instance.camera.postProcessing));
    }
    public void SetPostProcessing(float value)
    {
        Instance.camera.postProcessing = Convert.ToBoolean(value);
        UpdatePostProcessing();
        SetSlider(postprocessSlider, Convert.ToInt32(Instance.camera.postProcessing));
    }
    public void UpdatePostProcessing()
    {
        foreach (Camera camera in mainCameras)
        {
            PostProcessLayer ppb = camera.GetComponent<PostProcessLayer>();
            if (ppb != null)
            {
                if (Instance.camera.deferredRendering)
                    ppb.enabled = Instance.camera.postProcessing;
                else
                    ppb.enabled = false;
            }
        }
    }
#endregion

#endregion

}
