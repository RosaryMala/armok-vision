// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace Boxophobic.StyledGUI
{
    public class StyledMaskDrawer : MaterialPropertyDrawer
    {
        public string file = "";
        public string options = "";

        public float top = 0;
        public float down = 0;

        public StyledMaskDrawer(string file, string options, float top, float down)
        {
            this.file = file;
            this.options = options;

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

            if (Resources.Load<TextAsset>(file) != null)
            {
                var layersPath = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>(file));

                StreamReader reader = new StreamReader(layersPath);

                options = reader.ReadLine();

                reader.Close();
            }

            string[] enumSplit = options.Split(char.Parse(" "));
            List<string> enumOptions = new List<string>(enumSplit.Length / 2);

            for (int i = 0; i < enumSplit.Length; i++)
            {
                if (i % 2 == 0)
                {
                    enumOptions.Add(enumSplit[i].Replace("_", " "));
                }
            }

            GUILayout.Space(top);

            int index = (int)prop.floatValue;

            index = EditorGUILayout.MaskField(prop.displayName, index, enumOptions.ToArray());

            if (index < 0)
            {
                index = -1;
            }

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
