using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Serializable]
    public class Meshing
    {
        public int meshingThreads = 2;
        public int queueLimit = 4;
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
        public bool drawDistantTerrain = true;
        public bool drawShadows = true;
        public int vSyncCount = 0;
        public int targetFrameRate = 60;
        public bool showHiddenTiles = false;
    }
    [Serializable]
    public class CameraSettings
    {
        public float fieldOfView = 70;
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
}
