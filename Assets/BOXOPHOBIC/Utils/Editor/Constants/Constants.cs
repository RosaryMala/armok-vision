//  Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.Constants
{
    public static class CONSTANT
    {
        public static Color CategoryColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return CONSTANT.ColorDarkGray;
                }
                else
                {
                    return CONSTANT.ColorLightGray;
                }
            }
        }

        public static Color LineColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return new Color(0.15f, 0.15f, 0.15f, 1.0f);
                }
                else
                {
                    return new Color(0.65f, 0.65f, 0.65f, 1.0f);
                }
            }
        }

        public static Color ColorDarkGray
        {
            get
            {
                return new Color(0.2f, 0.2f, 0.2f, 1.0f);
            }
        }

        public static Color ColorLightGray
        {
            get
            {
                return new Color(0.82f, 0.82f, 0.82f, 1.0f);
            }
        }

        public static GUIStyle TitleStyle
        {
            get
            {
                GUIStyle guiStyle = new GUIStyle("label")
                {
                    richText = true,
                    alignment = TextAnchor.MiddleCenter
                };

                return guiStyle;
            }
        }

        public static GUIStyle HeaderStyle
        {
            get
            {
                GUIStyle guiStyle = new GUIStyle("label")
                {
                    richText = true,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft
                };

                return guiStyle;
            }
        }
    }
}

