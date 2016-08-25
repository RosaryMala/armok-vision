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
        BuildRelease(BuildTarget.StandaloneOSXUniversal);
        BuildRelease(BuildTarget.StandaloneLinuxUniversal);
        BuildRelease(BuildTarget.StandaloneWindows);
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
            case BuildTarget.StandaloneOSXUniversal:
                releaseName = buildSettings.osx_exe;
                targetString = "Mac";
                break;
            case BuildTarget.StandaloneLinuxUniversal:
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

        string[] levels = new string[] { "Assets/Start.unity" };
        EditorUserBuildSettings.SetPlatformSettings("Standalone", "CopyPDBFiles", "false");
        Debug.Log(BuildPipeline.BuildPlayer(levels, path + releaseName, target, BuildOptions.None));
        CopyExtras(path);
        File.WriteAllText(path + "manifest.json", JsonConvert.SerializeObject(buildSettings, Formatting.Indented));

        using (ZipFile zip = new ZipFile())
        {
            zip.AddDirectory(path);
            zip.Save("Build/" + buildSettings.title + " " + buildSettings.content_version + " " + targetString + ".zip");
        }
    }

    static void CopyExtras(string path)
    {
        FileUtil.ReplaceFile("ReleaseFiles/Changelog.txt", path + "Changelog.txt");
        FileUtil.ReplaceFile("ReleaseFiles/Credits.txt", path + "Credits.txt");
        FileUtil.ReplaceFile("ReleaseFiles/Readme.txt", path + "Readme.txt");
        FileUtil.ReplaceDirectory("ReleaseFiles/Plugins/", path + "Plugins");
    }
}
