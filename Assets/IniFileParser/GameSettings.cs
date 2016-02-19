using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

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
        public int maxBlocksToDraw = int.MaxValue;
        public int maxTextureSize = 512;
        public int textureAtlasSize = 2048;
        public bool debugTextureAtlas = false;
        public bool drawClouds = true;
        public bool drawDistantTerrain = true;
        public bool drawShadows = true;
        public int vSyncCount = 0;
        public int targetFrameRate = 60;
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
    public class Settings
    {
        public Meshing meshing = new Meshing();
        public Rendering rendering = new Rendering();
        public Units units = new Units();
        public CameraSettings camera = new CameraSettings();
    }

    public static Settings Instance { get; private set; }

    public Settings localInstance = new Settings();

    public Camera mainCamera;

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
        Instance = localInstance;
        Instance.camera.fieldOfView = mainCamera.fieldOfView;
        DeserializeIni("Config.json");
        mainCamera.fieldOfView = Instance.camera.fieldOfView;
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
