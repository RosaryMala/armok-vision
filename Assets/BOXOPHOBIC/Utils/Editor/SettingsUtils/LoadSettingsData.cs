// Cristian Pop - https://boxophobic.com/

using System.Globalization;
using UnityEditor;

namespace Boxophobic.Utils
{
    public partial class SettingsUtils
    {
        public static string LoadSettingsData(string settingsPath, string defaultData)
        {
            var settings = AssetDatabase.LoadAssetAtPath<SettingsData>(settingsPath);

            if (settings != null)
            {
                return settings.data;
            }
            else
            {
                return defaultData;
            }
        }

        public static int LoadSettingsData(string settingsPath, int defaultData)
        {
            var settings = AssetDatabase.LoadAssetAtPath<SettingsData>(settingsPath);

            if (settings != null)
            {
                int value;

                if (int.TryParse(settings.data, out value))
                {
                    return value;
                }
                else
                {
                    return defaultData;
                }
            }
            else
            {
                return defaultData;
            }
        }

        public static float LoadSettingsData(string settingsPath, float defaultData)
        {
            var settings = AssetDatabase.LoadAssetAtPath<SettingsData>(settingsPath);

            if (settings != null)
            {
                float value;

                if (float.TryParse(settings.data, out value))
                {
                    return float.Parse(settings.data, CultureInfo.InvariantCulture);
                }
                else
                {
                    return defaultData;
                }
            }
            else
            {
                return defaultData;
            }
        }
    }
}

