// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledMask))]
    public class StyledMaskAttributeDrawer : PropertyDrawer
    {
        StyledMask a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledMask)attribute;

            GUIStyle styleLabel = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            if (Resources.Load<TextAsset>(a.file) != null)
            {
                var layersPath = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>(a.file));

                StreamReader reader = new StreamReader(layersPath);

                a.options = reader.ReadLine();

                reader.Close();
            }

            string[] enumSplit = a.options.Split(char.Parse(" "));
            List<string> enumOptions = new List<string>(enumSplit.Length / 2);

            for (int i = 0; i < enumSplit.Length; i++)
            {
                if (i % 2 == 0)
                {
                    enumOptions.Add(enumSplit[i].Replace("_", " "));
                }
            }

            GUILayout.Space(a.top);

            int index = property.intValue;

            if (a.display == "")
            {
                a.display = property.displayName;
            }

            index = EditorGUILayout.MaskField(a.display, index, enumOptions.ToArray());

            if (Mathf.Abs(index) > 32000)
            {
                index = -1;
            }

            //Debug Value
            EditorGUILayout.LabelField(index.ToString());

            property.intValue = index;

            GUILayout.Space(a.down);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }
    }
}
