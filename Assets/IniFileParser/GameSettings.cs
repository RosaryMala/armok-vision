using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public enum LandscapeDetail
    {
        Off,
        Low,
        High
    }
    [Serializable]
    public class Meshing
    {
        public int meshingThreads = 4;
        public int queueLimit = 8;
    }

    [Serializable]
    public class Rendering
    {
        public int drawRangeSide = 4;
        public int drawRangeUp = 1;
        public int drawRangeDown = 5;
        public int maxBlocksToDraw = 460800;
        public int maxTextureSize = 512;
        public int textureAtlasSize = 2048;
        public bool debugTextureAtlas = false;
        public bool drawClouds = true;
        public LandscapeDetail distantTerrainDetail = LandscapeDetail.High;
        public bool drawShadows = true;
        public int vSyncCount = 0;
        public int targetFrameRate = 60;
        public bool showHiddenTiles = false;
        public bool fog = true;
    }
    [Serializable]
    public class CameraSettings
    {
        public float fieldOfView = 70;
        public bool deferredRendering = true;
        public bool SSAO = true;
        public bool postProcessing = true;
    }

    [Serializable]
    public class Units
    {
        public bool drawUnits = true;
        public bool scaleUnits = true;
    }

    [Serializable]
    public class Game
    {
        public bool showDFScreen = false;
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
    public class Settings
    {
        public Meshing meshing = new Meshing();
        public Rendering rendering = new Rendering();
        public Units units = new Units();
        public CameraSettings camera = new CameraSettings();
        public Game game = new Game();
        public Debug debug = new Debug();
    }

    public static Settings Instance = new Settings();

    public List<Camera> mainCameras;

    public Light[] LightList;

    static void DeserializeIni(string filename)
    {
        if (!File.Exists(filename))
            return;

        Instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(filename));
    }

    static void SerializeIni(string filename)
    { 
        File.WriteAllText(filename, JsonConvert.SerializeObject(Instance, Formatting.Indented));
    }

    // This function is called when the MonoBehaviour will be destroyed
    public void OnDestroy()
    {
        SerializeIni("Config.json");
    }

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        Instance.camera.fieldOfView = mainCameras[0].fieldOfView;
        DeserializeIni("Config.json");
        foreach (Camera camera in mainCameras)
        {
            camera.fieldOfView = Instance.camera.fieldOfView;
        }
        SetShadows(Instance.rendering.drawShadows);
        Application.targetFrameRate = Instance.rendering.targetFrameRate;
        QualitySettings.vSyncCount = Instance.rendering.vSyncCount;

        UpdatePostProcessing();
    }

    void SetShadows(bool input)
    {
        foreach (Light item in LightList)
        {
            switch (input)
            {
                case true:
                    item.shadows = LightShadows.Hard;
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
        UnityEngine.Debug.Log("Invoked from " + go);
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
        UnityEngine.Debug.Log("Invoked from " + go);
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
            PostProcessingBehaviour ppb = camera.GetComponent<PostProcessingBehaviour>();
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
