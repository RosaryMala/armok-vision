// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace Boxophobic.StyledGUI
{
    public class StyledLayersDrawer : MaterialPropertyDrawer
    {
        public float top = 0;
        public float down = 0;

        public StyledLayersDrawer()
        {

        }

        public StyledLayersDrawer(float top, float down)
        {
            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            GUIStyle styleLabel = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            int index = (int)prop.floatValue;

            string[] allLayers = new string[32];

            for (int i = 0; i < 32; i++)
            {
                if (LayerMask.LayerToName(i).Length < 1)
                {
                    allLayers[i] = "Missing";
                }
                else
                {
                    allLayers[i] = LayerMask.LayerToName(i);
                }
            }

            GUILayout.Space(top);

            index = EditorGUILayout.MaskField(prop.displayName, index, allLayers);

            //if (index < 0)
            //{
            //    index = -1;
            //}

            //Debug Value
            //EditorGUILayout.LabelField(index.ToString());

            prop.floatValue = index;

            GUILayout.Space(down);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}
