using UnityEditor;
using UnityEngine;
using Ionic.Zip;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

public class BuildFactory
{
    [MenuItem("Mytools/Build Release")]
    public static void BuildAll()
    {
        BuildRelease(BuildTarget.StandaloneOSXIntel64);
        BuildRelease(BuildTarget.StandaloneLinux64);
        BuildRelease(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Mytools/Build Windows Release")]
    public static void BuildWin()
    {
        BuildRelease(BuildTarget.StandaloneWindows64);
    }

    static void BuildRelease(BuildTarget target)
    {


        string targetString = "";
        string releaseName = "";

        switch (target)
        {
            case BuildTarget.StandaloneOSXIntel64:
                releaseName = BuildSettings.Instance.osx_exe;
                targetString = "Mac";
                break;
            case BuildTarget.StandaloneLinux64:
                releaseName = BuildSettings.Instance.linux_exe;
                targetString = "Linux";
                break;
            case BuildTarget.StandaloneWindows:
                releaseName = BuildSettings.Instance.win_exe;
                targetString = "Win";
                break;
            case BuildTarget.StandaloneWindows64:
                releaseName = BuildSettings.Instance.win_exe;
                targetString = "Win x64";
                break;
            default:
                break;
        }

        string path = "Build/" + BuildSettings.Instance.content_version + "/" + targetString + "/";

        if (Directory.Exists(path))
            Directory.Delete(path, true);

        string[] levels = new string[] { "Assets/Start.unity" };
        EditorUserBuildSettings.SetPlatformSettings("Standalone", "CopyPDBFiles", "false");
        UnityEngine.Debug.Log(BuildPipeline.BuildPlayer(levels, path + releaseName, target, BuildOptions.None));
        CopyExtras(path);

        using (ZipFile zip = new ZipFile())
        {
            zip.AddDirectory(path);
            zip.Save("Build/" + BuildSettings.Instance.title + " " + BuildSettings.Instance.content_version + " " + targetString + ".zip");
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

    [MenuItem("Mytools/Build Proto")]
    public static void BuildProto()
    {
        File.Copy("D:\\Home\\Documents\\GitHub\\dfhack\\plugins\\proto\\RemoteFortressReader.proto", "Assets\\RemoteClientDF\\RemoteFortressReader.proto", true);
        Process protogen = new Process();

        protogen.StartInfo.WorkingDirectory = "Assets\\RemoteClientDF\\";
        protogen.StartInfo.FileName = "ProtoGen\\protogen.exe";
        protogen.StartInfo.Arguments = "-i:RemoteFortressReader.proto -o:RemoteFortressReader.cs";

        //redirect output
        protogen.StartInfo.RedirectStandardError = true;
        protogen.StartInfo.RedirectStandardOutput = true;

        protogen.OutputDataReceived += (sender, args) => { if (args.Data != null) UnityEngine.Debug.Log(args.Data); };
        protogen.ErrorDataReceived += (sender, args) => { if (args.Data != null) UnityEngine.Debug.LogError(args.Data); };

        protogen.StartInfo.UseShellExecute = false;
        protogen.StartInfo.CreateNoWindow = true;

        protogen.Start();

        protogen.BeginOutputReadLine();
        protogen.BeginErrorReadLine();

        protogen.WaitForExit();

        UnityEngine.Debug.Log("Finished compiling protos");
    }
}
