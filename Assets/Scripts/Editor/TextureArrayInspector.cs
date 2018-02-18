using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureArrayInspector : Editor
{
    enum TextureType
    {
        Albedo,
        Specular,
        Normal,
        Height,
        AO,
        R,G,B,A
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Texture2DArray))]
    class TextureInspector : Editor
    {
        TextureType previewType = TextureType.Albedo;
        Material previewMat;
        int index = 0;
        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewSettings()
        {
            index = EditorGUILayout.IntSlider(index, 0, (target as Texture2DArray).depth - 1, new GUILayoutOption[] { GUILayout.MaxWidth(128f) });
            var type = (TextureType)EditorGUILayout.EnumPopup(previewType, new GUILayoutOption[] { GUILayout.MaxWidth(64f) });
            if (previewMat == null)
                previewMat = new Material(Shader.Find("Hidden/TexArrayBlitAlbedo"));
            if(type != previewType)
            {
                previewType = type;
                switch (type)
                {
                    case TextureType.Albedo:
                        previewMat.shader = Shader.Find("Hidden/TexArrayBlitAlbedo");
                        break;
                    case TextureType.Specular:
                        previewMat.shader = Shader.Find("Hidden/TexArrayBlitAlpha");
                        break;
                    case TextureType.Normal:
                        previewMat.shader = Shader.Find("Hidden/TexArrayBlitNormal");
                        break;
                    case TextureType.Height:
                        previewMat.shader = Shader.Find("Hidden/TexArrayBlitBlue");
                        break;
                    case TextureType.AO:
                        previewMat.shader = Shader.Find("Hidden/TexArrayBlitRed");
                        break;
                    case TextureType.R:
                        previewMat.shader = Shader.Find("Hidden/TexArrayBlitRed");
                        break;
                    case TextureType.G:
                        previewMat.shader = Shader.Find("Hidden/TexArrayBlitGreen");
                        break;
                    case TextureType.B:
                        previewMat.shader = Shader.Find("Hidden/TexArrayBlitBlue");
                        break;
                    case TextureType.A:
                        previewMat.shader = Shader.Find("Hidden/TexArrayBlitAlpha");
                        break;
                    default:
                        break;
                }
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (previewMat == null)
                previewMat = new Material(Shader.Find("Hidden/TexArrayBlitAlbedo"));
            base.OnPreviewGUI(r, background);
            previewMat.SetFloat("_Index", index);
            EditorGUI.DrawPreviewTexture(r, target as Texture2DArray, previewMat, ScaleMode.ScaleToFit);
        }
    }
}
