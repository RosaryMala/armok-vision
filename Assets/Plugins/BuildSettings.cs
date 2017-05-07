using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
[CreateAssetMenu]
public class BuildSettings : ScriptableObject
{
    [JsonProperty]
    public string author;
    [JsonProperty]
    public string content_version;
    [JsonProperty]
    public string df_min_version;
    [JsonProperty]
    public string df_max_version;
    [JsonProperty]
    public string title;
    [JsonProperty]
    public string tooltip;
    [JsonProperty]
    public string win_exe;
    [JsonProperty]
    public string osx_exe;
    [JsonProperty]
    public string linux_exe;
    [JsonProperty]
    public bool launch_with_terminal;
    [JsonProperty]
    public string readme;
    [JsonProperty]
    public bool needs_dfhack;

    private static BuildSettings _instance = null;
    public static BuildSettings Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<BuildSettings>("Build Settings");
            if (_instance == null)
            {
                _instance = CreateInstance<BuildSettings>();
                _instance.author = "MISSING_MANIFEST";
                _instance.content_version = "MISSING_MANIFEST";
                _instance.df_min_version = "MISSING_MANIFEST";
                _instance.df_max_version = "MISSING_MANIFEST";
                _instance.title = "MISSING_MANIFEST";
                _instance.tooltip = "MISSING_MANIFEST";
                _instance.win_exe = "MISSING_MANIFEST";
                _instance.osx_exe = "MISSING_MANIFEST";
                _instance.linux_exe = "MISSING_MANIFEST";
                _instance.launch_with_terminal = false;
                _instance.readme = "MISSING_MANIFEST";
                _instance.needs_dfhack = true;
            }
            return _instance;
        }
    }
}
