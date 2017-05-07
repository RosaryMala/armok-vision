using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class BuildManifest {
    public string scmCommitId;
    public string scmBranch;
    public string buildNumber;
    public string buildStartTime;
    public string projectId;
    public string bundleId;
    public string unityVersion;
    public string xcodeVersion;
    public string cloudBuildTargetName;

    private static BuildManifest _instance = null;
    public static BuildManifest Instance
    {
        get
        {
            if (_instance == null)
            {
                var unityCloudBuildManifest = (TextAsset)Resources.Load("UnityCloudBuildManifest.json");
                if (unityCloudBuildManifest == null)
                {
                    _instance = new BuildManifest();
                    _instance.scmCommitId = "LOCAL_BUILD";
                    _instance.scmBranch = "LOCAL_BUILD";
                    _instance.buildNumber = "LOCAL_BUILD";
                    _instance.buildStartTime = "LOCAL_BUILD";
                    _instance.projectId = "LOCAL_BUILD";
                    _instance.bundleId = "LOCAL_BUILD";
                    _instance.unityVersion = "LOCAL_BUILD";
                    _instance.xcodeVersion = "LOCAL_BUILD";
                    _instance.cloudBuildTargetName = "LOCAL_BUILD";
                }
                else
                    _instance = JsonConvert.DeserializeObject<BuildManifest>(unityCloudBuildManifest.text);
            }
            return _instance;
        }
    }
}
