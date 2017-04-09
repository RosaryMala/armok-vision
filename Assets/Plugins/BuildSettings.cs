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
}
