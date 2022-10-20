// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledTexturePreview))]
    public class StyledTexturePreviewAttributeDrawer : PropertyDrawer
    {
        int channel = 0;
        ColorWriteMask channelMask = ColorWriteMask.All;

        StyledTexturePreview a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledTexturePreview)attribute;

            var tex = (Texture)property.objectReferenceValue;

            if (a.displayName != "")
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(-1);
                GUILayout.Label(a.displayName, GUILayout.Width(EditorGUIUtility.labelWidth - 1));
                tex = (Texture)EditorGUILayout.ObjectField(tex, typeof(Texture), false);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                property.objectReferenceValue = tex;
            }

            if (tex == null)
            {
                return;
            }

            var styledText = new GUIStyle(EditorStyles.toolbarButton)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal,
                fontSize = 10,
            };

            var styledPopup = new GUIStyle(EditorStyles.toolbarPopup)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
            };

            var rect = GUILayoutUtility.GetRect(0, 0, Screen.width, 0);

            EditorGUI.DrawPreviewTexture(rect, tex, null, ScaleMode.ScaleAndCrop, 1, 0, channelMask);

            GUILayout.Space(2);

            GUILayout.BeginHorizontal();

            GUILayout.Label((UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(tex) / 1024f / 1024f).ToString("F2") + " mb", styledText);
            GUILayout.Space(-1);
            GUILayout.Label(tex.width.ToString(), styledText);
            GUILayout.Space(-1);
            GUILayout.Label(tex.graphicsFormat.ToString(), styledText);
            GUILayout.Space(-1);

            channel = EditorGUILayout.Popup(channel, new string[] { "RGB", "R", "G", "B", "A" }, styledPopup, GUILayout.MaxWidth(60)); 

            GUILayout.EndHorizontal();

            if (channel == 0)
            {
                channelMask = ColorWriteMask.All;
            }
            else if (channel == 1)
            {
                channelMask = ColorWriteMask.Red;
            }
            else if (channel == 2)
            {
                channelMask = ColorWriteMask.Green;
            }
            else if (channel == 3)
            {
                channelMask = ColorWriteMask.Blue;
            }
            else if (channel == 4)
            {
                channelMask = ColorWriteMask.Alpha;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }
    }
}
