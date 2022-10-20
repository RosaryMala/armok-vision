// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.Utils
{
    [CreateAssetMenu(fileName = "Data", menuName = "BOXOPHOBIC/Settings Data")]
    public class SettingsData : ScriptableObject
    {
        [Space]
        public string data = "";
    }
}