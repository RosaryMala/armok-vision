// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace Boxophobic.StyledGUI
{
    public class StyledEnumDrawer : MaterialPropertyDrawer
    {
        public string file = "";
        public string options = "";

        public float top = 0;
        public float down = 0;

        public StyledEnumDrawer(string file, string options, float top, float down)
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
            List<int> enumIndices = new List<int>(enumSplit.Length / 2);

            for (int i = 0; i < enumSplit.Length; i++)
            {
                if (i % 2 == 0)
                {
                    enumOptions.Add(enumSplit[i].Replace("_", " "));
                }
                else
                {
                    enumIndices.Add(int.Parse(enumSplit[i]));
                }
            }

            GUILayout.Space(top);

            int index = (int)prop.floatValue;
            int realIndex = enumIndices[0];

            for (int i = 0; i < enumIndices.Count; i++)
            {
                if (enumIndices[i] == index)
                {
                    realIndex = i;
                }
            }

            realIndex = EditorGUILayout.Popup(prop.displayName, realIndex, enumOptions.ToArray());

            //Debug Value
            //EditorGUILayout.LabelField(enumIndices[realIndex].ToString());

            prop.floatValue = enumIndices[realIndex];

            GUILayout.Space(down);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}
