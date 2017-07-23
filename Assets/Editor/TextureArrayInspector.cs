using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureArrayInspector : Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Texture2DArray))]
    class TextureInspector : Editor
    {
        Material previewMat;
        float index = 0;
        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewSettings()
        {
            index = GUILayout.HorizontalSlider(index, 0, (target as Texture2DArray).depth - 1, new GUILayoutOption[]
                {
                    GUILayout.MaxWidth(64f)
                });
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (previewMat == null)
                previewMat = new Material(Shader.Find("Hidden/TexArrayBlit"));
            base.OnPreviewGUI(r, background);
            previewMat.SetFloat("_Index", index);
            EditorGUI.DrawPreviewTexture(r, target as Texture2DArray, previewMat);
        }
    }
}
