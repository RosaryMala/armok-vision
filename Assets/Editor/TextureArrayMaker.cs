using hqx;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureArrayMaker : EditorWindow
{
    enum ScaleMode
    {
        None,
        HQ2x,
        HQ3x,
        HQ4x
    }

    Texture2D baseTexture;
    int tiles_x = 16;
    int tiles_y = 16;
    ScaleMode scaleMode = ScaleMode.None;
    bool mipmaps = true;
    Texture2DArray texArray;
    int previewIndex = 0;
    Texture2D previewTexture;

    [MenuItem("Mytools/Texture Array Builder")]
    public static void BuildTextureArray()
    {
        TextureArrayMaker window = GetWindow<TextureArrayMaker>();
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Texure array maker", EditorStyles.boldLabel);
        baseTexture = (Texture2D)EditorGUILayout.ObjectField("Base texture image", baseTexture, typeof(Texture2D), false);
        tiles_x = EditorGUILayout.IntField("Horizontal Tiles", tiles_x);
        tiles_y = EditorGUILayout.IntField("Vertical Tiles", tiles_y);
        if (tiles_x <= 0) tiles_x = 1;
        if (tiles_y <= 0) tiles_y = 1;

        mipmaps = EditorGUILayout.Toggle("Enable Mipmaps", mipmaps);

        scaleMode = (ScaleMode)EditorGUILayout.EnumPopup("Scale mode", scaleMode);
        int scale = 1;
        switch (scaleMode)
        {
            case ScaleMode.None:
                scale = 1;
                break;
            case ScaleMode.HQ2x:
                scale = 2;
                break;
            case ScaleMode.HQ3x:
                scale = 3;
                break;
            case ScaleMode.HQ4x:
                scale = 4;
                break;
            default:
                break;
        }
        if (baseTexture != null)
        {
            if (GUILayout.Button("Build Array"))
            {
                int sourceWidth = baseTexture.width / tiles_x;
                int sourceHeight = baseTexture.height / tiles_y;
                texArray = new Texture2DArray(scale * sourceWidth, scale * sourceHeight, tiles_x * tiles_y, TextureFormat.ARGB32, mipmaps);
                var tempTex = new Texture2D(baseTexture.width / tiles_x, baseTexture.height / tiles_y, TextureFormat.ARGB32, false);
                int i = 0;
                for (int y = tiles_y-1; y >= 0 ; y--)
                    for (int x = 0; x < tiles_x; x++)
                    {
                        tempTex.Resize(baseTexture.width / tiles_x, baseTexture.height / tiles_y);
                        tempTex.SetPixels(baseTexture.GetPixels(sourceWidth * x, sourceHeight * y, sourceWidth, sourceHeight));
                        switch (scaleMode)
                        {
                            case ScaleMode.HQ2x:
                                HqxSharp.Scale2(tempTex);
                                break;
                            case ScaleMode.HQ3x:
                                HqxSharp.Scale3(tempTex);
                                break;
                            case ScaleMode.HQ4x:
                                HqxSharp.Scale4(tempTex);
                                break;
                            default:
                                break;
                        }
                        texArray.SetPixels(tempTex.GetPixels(), i);
                        i++;
                    }
                texArray.Apply();
                texArray.name = baseTexture.name + "Array";
            }
        }

        texArray = (Texture2DArray)EditorGUILayout.ObjectField("Texture Array", texArray, typeof(Texture2DArray), true);

        if (texArray != null)
        {
            previewIndex = EditorGUILayout.IntSlider(previewIndex, 0, texArray.depth - 1);
            if (previewTexture == null)
                previewTexture = new Texture2D(texArray.width, texArray.height, TextureFormat.ARGB32, false);
            if (previewTexture.width != texArray.width || previewTexture.height != texArray.height)
                previewTexture.Resize(texArray.width, texArray.height);

            var previewPixels = texArray.GetPixels(previewIndex);

            for (int i = 0; i < previewPixels.Length; i++)
            {
                previewPixels[i] = Color.Lerp(Color.magenta, previewPixels[i], previewPixels[i].a);
            }

            previewTexture.SetPixels(previewPixels);
            previewTexture.Apply();

            EditorGUI.DrawPreviewTexture(EditorGUILayout.GetControlRect(false, previewTexture.height), previewTexture, null, UnityEngine.ScaleMode.ScaleToFit);

            if(GUILayout.Button("Save Texture Array"))
            {
                var path = EditorUtility.SaveFilePanelInProject("Save texture array to asset", texArray.name + ".asset", "asset", "Please select a filename to save the texture atlas to.");
                AssetDatabase.CreateAsset(texArray, path);
                AssetDatabase.Refresh();
            }
        }
    }
}
