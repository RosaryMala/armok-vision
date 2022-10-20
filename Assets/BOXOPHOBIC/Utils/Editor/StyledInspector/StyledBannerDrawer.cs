// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledBanner))]
    public class StyledBannerAttributeDrawer : PropertyDrawer
    {
        StyledBanner a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledBanner)attribute;

            var bannerColor = new Color(a.colorR, a.colorG, a.colorB);

            StyledGUI.DrawInspectorBanner(bannerColor, a.title);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }
    }
}
