using UnityEditor;
using UnityEngine;
using Ionic.Zip;

public class BuildFactory
{
    const string releaseName = "Armok Vision";

    const string version = "0.11.1";

    [MenuItem("Mytools/Build Release")]
    public static void BuildAll()
    {
        BuildRelease(BuildTarget.StandaloneWindows);
        BuildRelease(BuildTarget.StandaloneWindows64);
        BuildRelease(BuildTarget.StandaloneOSXUniversal);
        BuildRelease(BuildTarget.StandaloneLinuxUniversal);
    }

    static void BuildRelease(BuildTarget target)
    {

        string ext = "";

        string targetString = "";

        switch (target)
        {
            case BuildTarget.StandaloneOSXUniversal:
                targetString = "Mac";
                break;
            case BuildTarget.StandaloneLinuxUniversal:
                targetString = "Linux";
                break;
            case BuildTarget.StandaloneWindows:
                targetString = "Win";
                break;
            case BuildTarget.StandaloneWindows64:
                targetString = "Win x64";
                break;
            default:
                break;
        }

        string path = "Build/" + targetString + "/";

        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            ext = ".exe";

        string[] levels = new string[] { "Assets/Start.unity" };

        Debug.Log(BuildPipeline.BuildPlayer(levels, path + releaseName + ext, target, BuildOptions.None));
        CopyExtras(path);

        using (ZipFile zip = new ZipFile())
        {
            zip.AddDirectory(path);
            zip.Save("Build/" + releaseName + " " + version + " " + targetString + ".zip");
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
