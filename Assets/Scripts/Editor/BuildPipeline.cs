using UnityEditor;
using UnityEngine;
using Ionic.Zip;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using UnityEngine.CloudBuild;
using MaterialStore;

public class BuildFactory
{
    [MenuItem("Mytools/Build Release/All")]
    public static void BuildAll()
    {
        BuildRelease(BuildTarget.StandaloneOSX);
        BuildRelease(BuildTarget.StandaloneLinux64);
        BuildRelease(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Mytools/Build Release/Windows")]
    public static void BuildWin()
    {
        BuildRelease(BuildTarget.StandaloneWindows64);
    }
    [MenuItem("Mytools/Build Release/Windows Debug")]
    public static void BuildWinDebug()
    {
        BuildRelease(BuildTarget.StandaloneWindows64, true);
    }

    [MenuItem("Mytools/Build Release/OSx")]
    public static void BuildOsx()
    {
        BuildRelease(BuildTarget.StandaloneOSX);
    }

    static void BuildRelease(BuildTarget target, bool isDebug = false)
    {
        MaterialCollector.BuildMaterialCollection();

        string targetString = "";
        string releaseName = "";
        BuildSettings.Instance.build_date = System.DateTime.Now.ToString("yyy-MM-dd");
        EditorUtility.SetDirty(BuildSettings.Instance);
        AssetDatabase.SaveAssets();

        switch (target)
        {
            case BuildTarget.StandaloneOSX:
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

        string path = "Build/" + (isDebug ? BuildSettings.Instance.build_date : BuildSettings.Instance.content_version) + "/" + targetString + (isDebug ? "_debug" : "") + "/" ;

        if (Directory.Exists(path))
            Directory.Delete(path, true);

        string[] levels = new string[] { "Assets/Scenes/Map Mode.unity" };
        EditorUserBuildSettings.SetPlatformSettings("Standalone", "CopyPDBFiles", "false");
        var options = BuildOptions.None;
        if (isDebug)
            options |= BuildOptions.AllowDebugging;
        UnityEngine.Debug.Log(BuildPipeline.BuildPlayer(levels, path + releaseName, target, options));
        CopyExtras(path);

        using (ZipFile zip = new ZipFile())
        {
            zip.AddDirectory(path);
            zip.Save("Build/" + BuildSettings.Instance.title + " " + (isDebug ? BuildSettings.Instance.build_date : BuildSettings.Instance.content_version) + " " + targetString + (isDebug ? "_debug" : "") + ".zip");
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
        CompileProtoFile("D:/Home/Documents/GitHub/dfhack/plugins/proto/RemoteFortressReader.proto");
        CompileProtoFile("D:/Home/Documents/GitHub/dfhack/plugins/proto/AdventureControl.proto");
        UnityEngine.Debug.Log("Finished compiling protos");
        AssetDatabase.Refresh();
    }

    static void CompileProtoFile(string path)
    {
        File.Copy(path, Path.Combine("Assets/RemoteClientDF/", Path.GetFileName(path)), true);
        Process protogen = new Process();

        protogen.StartInfo.WorkingDirectory = "Assets/RemoteClientDF/";
        protogen.StartInfo.FileName = "ProtoGen/protogen.exe";
        protogen.StartInfo.Arguments = string.Format("-i:{0}.proto -o:{0}.cs", Path.GetFileNameWithoutExtension(path));

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
    }

    public static void PreBuild(BuildManifestObject manifest)
    {
        MaterialCollector.BuildMaterialCollection();
        RenderTexture.active = null; //Attempt at cloud build error fixing.
    }
}
