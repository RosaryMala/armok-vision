// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledTextDrawer : MaterialPropertyDrawer
    {
        public string text = "";
        public string alignment = "Center";
        public string font = "Normal";
        public float size = 11;
        public float top = 0;
        public float down = 0;

        public StyledTextDrawer(string text)
        {
            this.text = text;
        }

        public StyledTextDrawer(string text, string alignment, string font, float size)
        {
            this.text = text;
            this.alignment = alignment;
            this.font = font;
            this.size = size;
        }

        public StyledTextDrawer(string text, string alignment, string font, float size, float top, float down)
        {
            this.text = text;
            this.alignment = alignment;
            this.font = font;
            this.size = size;
            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            //Material material = materialEditor.target as Material;

            GUIStyle styleLabel = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            GUILayout.Space(top);

            if (alignment == "Center")
            {
                styleLabel.alignment = TextAnchor.MiddleCenter;

            }
            else if (alignment == "Left")
            {
                styleLabel.alignment = TextAnchor.MiddleLeft;
            }
            else if (alignment == "Right")
            {
                styleLabel.alignment = TextAnchor.MiddleRight;
            }

            if (font == "Normal")
            {
                styleLabel.fontStyle = FontStyle.Normal;
            }
            else if (font == "Bold")
            {
                styleLabel.fontStyle = FontStyle.Bold;
            }
            else if (font == "Italic")
            {
                styleLabel.fontStyle = FontStyle.Italic;
            }
            else if (font == "BoldAndItalic")
            {
                styleLabel.fontStyle = FontStyle.BoldAndItalic;
            }

            styleLabel.fontSize = (int)size;

            GUILayout.Label(text, styleLabel);

            GUILayout.Space(down);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}
