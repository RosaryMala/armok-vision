using UnityEditor;
using UnityEngine;
using Ionic.Zip;
using Newtonsoft.Json;
using System.IO;

public class BuildFactory
{
    [MenuItem("Mytools/Build Release")]
    public static void BuildAll()
    {
        BuildRelease(BuildTarget.StandaloneWindows64);
        BuildRelease(BuildTarget.StandaloneOSXIntel64);
        BuildRelease(BuildTarget.StandaloneLinux64);
    }

    [MenuItem("Mytools/Build Windows Release")]
    public static void BuildWin()
    {
        BuildRelease(BuildTarget.StandaloneWindows64);
    }

    static void BuildRelease(BuildTarget target)
    {
        BuildSettings buildSettings = Resources.Load("Build Settings", typeof(BuildSettings)) as BuildSettings;

        if(buildSettings == null)
        {
            Debug.LogError("Can't find build settings");
            return;
        }

        string targetString = "";
        string releaseName = "";

        switch (target)
        {
            case BuildTarget.StandaloneOSXIntel64:
                releaseName = buildSettings.osx_exe;
                targetString = "Mac";
                break;
            case BuildTarget.StandaloneLinux64:
                releaseName = buildSettings.linux_exe;
                targetString = "Linux";
                break;
            case BuildTarget.StandaloneWindows:
                releaseName = buildSettings.win_exe;
                targetString = "Win";
                break;
            case BuildTarget.StandaloneWindows64:
                releaseName = buildSettings.win_exe;
                targetString = "Win x64";
                break;
            default:
                break;
        }

        string path = "Build/" + buildSettings.content_version + "/" + targetString + "/";

        if (Directory.Exists(path))
            Directory.Delete(path, true);

        string[] levels = new string[] { "Assets/Start.unity" };
        EditorUserBuildSettings.SetPlatformSettings("Standalone", "CopyPDBFiles", "false");
        Debug.Log(BuildPipeline.BuildPlayer(levels, path + releaseName, target, BuildOptions.None));
        CopyExtras(path);

        using (ZipFile zip = new ZipFile())
        {
            zip.AddDirectory(path);
            zip.Save("Build/" + buildSettings.title + " " + buildSettings.content_version + " " + targetString + ".zip");
        }
    }

    static void CopyExtras(string path)
    {
        path = Path.GetDirectoryName(path) + "/";
        BuildSettings buildSettings = Resources.Load("Build Settings", typeof(BuildSettings)) as BuildSettings;
        FileUtil.ReplaceFile("ReleaseFiles/Changelog.txt", path + "Changelog.txt");
        FileUtil.ReplaceFile("ReleaseFiles/Credits.txt", path + "Credits.txt");
        FileUtil.ReplaceFile("ReleaseFiles/Readme.txt", path + "Readme.txt");
        FileUtil.ReplaceDirectory("ReleaseFiles/Plugins/", path + "Plugins");
        File.WriteAllText(path + "manifest.json", JsonConvert.SerializeObject(buildSettings, Formatting.Indented));
    }
}
