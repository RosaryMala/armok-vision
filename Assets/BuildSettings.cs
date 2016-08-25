using UnityEngine;

[CreateAssetMenu]
public class BuildSettings : ScriptableObject {
    public string author;
    public string content_version;
    public string df_min_version;
    public string df_max_version;
    public string title;
    public string tooltip;
    public string win_exe;
    public string osx_exe;
    public string linux_exe;
    public bool launch_with_terminal;
    public string readme;
    public bool needs_dfhack;
}
