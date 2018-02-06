using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteFixer
{
    [MenuItem("Mytools/Fix Semitransparent Sprites")]
    public static void FixTransparentTextures()
    {
        var path = EditorUtility.OpenFolderPanel("Pick Sprite Folder", "", "SpriteFolder");
        var files = Directory.GetFiles(path, "*.png");
        foreach (var file in files)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
            tex.LoadImage(File.ReadAllBytes(file));
            var pixels = tex.GetPixels();
            float maxAlpha = 0;
            for(int i = 0; i < pixels.Length; i++)
            {
                maxAlpha = Mathf.Max(pixels[i].a, maxAlpha);
            }
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].a /= maxAlpha;
            }
            tex.SetPixels(pixels);
            File.WriteAllBytes(file, tex.EncodeToPNG());
        }
    }
}
