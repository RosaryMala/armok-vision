// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledVectorDrawer : MaterialPropertyDrawer
    {
        public float space = 0;
        public float top = 0;
        public float down = 0;

        public StyledVectorDrawer(float space)
        {
            this.space = space;
        }

        public StyledVectorDrawer(float space, float top, float down)
        {
            this.space = space;
            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            GUILayout.Space(top);

            if (EditorGUIUtility.currentViewWidth > 344)
            {
                materialEditor.VectorProperty(prop, label);
                GUILayout.Space(-space);
            }
            else
            {
                materialEditor.VectorProperty(prop, label);
                GUILayout.Space(2);
            }

            GUILayout.Space(down);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}
