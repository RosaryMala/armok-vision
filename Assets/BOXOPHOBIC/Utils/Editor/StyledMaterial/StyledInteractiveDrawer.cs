// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledInteractiveDrawer : MaterialPropertyDrawer
    {
        public StyledInteractiveDrawer()
        {

        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            if (prop.floatValue > 0.5f)
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}