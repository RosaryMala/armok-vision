// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.Constants;

namespace Boxophobic.StyledGUI
{
    public partial class StyledGUI
    {
        public static void DrawWindowBanner(Color color, string title)
        {
            GUILayout.Space(15);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(1, fullRect.position.y, fullRect.xMax - 2, 36);
            var lineRect = new Rect(1, fullRect.position.y, fullRect.xMax - 2, 1);

            if (EditorGUIUtility.isProSkin)
            {
                color = new Color(color.r, color.g, color.b, 1f);
            }
            else
            {
                color = CONSTANT.ColorLightGray;
            }

            EditorGUI.DrawRect(fillRect, color);
            EditorGUI.DrawRect(lineRect, CONSTANT.LineColor);

            Color guiColor = CONSTANT.ColorDarkGray;

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", CONSTANT.TitleStyle);

            GUILayout.Space(15);
        }

        public static void DrawWindowBanner(string title)
        {
            GUILayout.Space(15);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(2, fullRect.position.y, fullRect.xMax - 4, 36);

            Color color;
            Color guiColor;

            if (EditorGUIUtility.isProSkin)
            {
                color = CONSTANT.ColorDarkGray;
                guiColor = CONSTANT.ColorLightGray;
            }
            else
            {
                color = CONSTANT.ColorLightGray;
                guiColor = CONSTANT.ColorDarkGray;
            }

            EditorGUI.DrawRect(fillRect, color);

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", CONSTANT.TitleStyle);

            GUILayout.Space(15);
        }

        // Legacy
        public static void DrawWindowBanner(Color color, string title, string help)
        {
            DrawWindowBanner(color, title);
        }

        public static void DrawWindowBanner(string title, string help)
        {
            DrawWindowBanner(title);
        }
    }
}

