using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class UpdateChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameSettings.Instance.game.checkForUpdates)
            StartCoroutine(CheckForUpdates());
    }

    GithubRelease latestRelease = null;

    IEnumerator CheckForUpdates()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://api.github.com/repos/japamala/armok-vision/releases"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Connection Error: " + webRequest.error);
            }
            else
            {
                var releases = JsonConvert.DeserializeObject<GithubRelease[]>(webRequest.downloadHandler.text);
                foreach (var release in releases)
                {
                    if (release.draft)
                        continue;
                    if (release.prerelease)
                        continue;
                    latestRelease = release;
                    break;
                }
                if (latestRelease == null)
                {
                    Debug.Log("No valid release.");
                    yield break;
                }
                try
                {
                    System.Version latestVersion = new System.Version(latestRelease.tag_name.TrimStart('v'));
                    System.Version currentVersion = new System.Version(BuildSettings.Instance.content_version);
                    if(latestVersion > currentVersion)
                    {
                        ModalPanel.Instance.Choice(string.Format("There is a new version of Armok Vision availabe for download.\r\n" +
                            "Current version: {0}\r\n" +
                            "Latest version: {1}\r\n" +
                            "Would you like to download it?", currentVersion, latestVersion), OpenRelease, NoEvent, DontAskAgain, "Yes", "No", "Don't ask again.");
                    }
                    else
                    {
                        Debug.Log("Armok Vision it up to date!");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    yield break;
                }
            }
        }
    }

    void OpenRelease()
    {
        if (latestRelease != null)
            Application.OpenURL(latestRelease.html_url);
    }

    //empty.
    void NoEvent()
    {
    }

    void DontAskAgain()
    {
        GameSettings.Instance.game.checkForUpdates = false;
    }
    [System.Serializable]
    public class GithubRelease
    {
        public string url;
        public string assets_url;
        public string upload_url;
        public string html_url;
        public string tag_name;
        public string target_commitish;
        public string name;
        public bool draft;
        public bool prerelease;
    }
}
