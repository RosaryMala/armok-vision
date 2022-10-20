// Cristian Pop - https://boxophobic.com/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Boxophobic.Utils
{
    public partial class SettingsUtils
    {
        public static void SaveSettingsData(string settingsPath, string data)
        {
            CreateFileIfMissing(settingsPath);

            var settings = AssetDatabase.LoadAssetAtPath<SettingsData>(settingsPath);

            settings.data = data;

            SaveFile(settingsPath);
        }

        public static void SaveSettingsData(string settingsPath, int data)
        {
            CreateFileIfMissing(settingsPath);

            var settings = AssetDatabase.LoadAssetAtPath<SettingsData>(settingsPath);

            settings.data = data.ToString();

            SaveFile(settingsPath);
        }

        public static void SaveSettingsData(string settingsPath, float data)
        {
            CreateFileIfMissing(settingsPath);

            var settings = AssetDatabase.LoadAssetAtPath<SettingsData>(settingsPath);

            settings.data = data.ToString();

            SaveFile(settingsPath);
        }

        private static void CreateFileIfMissing(string settingsPath)
        {
            if (File.Exists(settingsPath) == false)
            {
                var directory = Path.GetDirectoryName(settingsPath);

                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                    AssetDatabase.Refresh();
                }

                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SettingsData>(), settingsPath);
                AssetDatabase.Refresh();
            }
        }

        private static void SaveFile(string settingsPath)
        {
            var file = AssetDatabase.LoadAssetAtPath<SettingsData>(settingsPath);

            EditorUtility.SetDirty(file);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

