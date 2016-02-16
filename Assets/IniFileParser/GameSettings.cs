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
        public int maxTextureSize = 512;
        public int textureAtlasSize = 2048;
        public bool debugTextureAtlas = false;
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
    }
}
