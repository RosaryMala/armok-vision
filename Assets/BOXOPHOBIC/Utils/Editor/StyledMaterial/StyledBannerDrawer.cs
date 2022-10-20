// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.Constants;

namespace Boxophobic.StyledGUI
{
    public class StyledBannerDrawer : MaterialPropertyDrawer
    {
        public string title;

        public StyledBannerDrawer(string title)
        {
            this.title = title;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor materialEditor)
        {
            StyledGUI.DrawInspectorBanner(title);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -4;
        }
    }
}
