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
    Texture2DArray normalArray;
    int previewIndex = 0;
    Texture2D previewTexture;
    Texture2D normalPreviewTexture;
    bool skipEmpty = false;

    [MenuItem("Mytools/Texture Array Builder")]
    public static void BuildTextureArray()
    {
        TextureArrayMaker window = GetWindow<TextureArrayMaker>();
        window.Show();
    }

    Vector2 scrollPosition;

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUILayout.Label("Texure array maker", EditorStyles.boldLabel);
        baseTexture = (Texture2D)EditorGUILayout.ObjectField("Base texture image", baseTexture, typeof(Texture2D), false);
        tiles_x = EditorGUILayout.IntField("Horizontal Tiles", tiles_x);
        tiles_y = EditorGUILayout.IntField("Vertical Tiles", tiles_y);
        if (tiles_x <= 0) tiles_x = 1;
        if (tiles_y <= 0) tiles_y = 1;

        mipmaps = EditorGUILayout.Toggle("Enable Mipmaps", mipmaps);
        skipEmpty = EditorGUILayout.Toggle("Skip Empty", skipEmpty);

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
            int sourceWidth = baseTexture.width / tiles_x;
            int sourceHeight = baseTexture.height / tiles_y;
            GUILayout.Label("Source Width: " + sourceWidth);
            GUILayout.Label("Source Height: " + sourceHeight);

            int scaledWidth = sourceWidth * scale;
            int scaledHeight = sourceHeight * scale;
            GUILayout.Label("Scaled Width: " + scaledWidth);
            GUILayout.Label("Scaled Height: " + scaledHeight);

            int potWidth = Mathf.ClosestPowerOfTwo(scaledWidth);
            int potHeight = Mathf.ClosestPowerOfTwo(scaledHeight);
            GUILayout.Label("Final Width: " + potWidth);
            GUILayout.Label("Final Height: " + potHeight);

            if (GUILayout.Button("Build Array"))
            {
                var tempList = new List<Texture2D>();
                for (int y = tiles_y-1; y >= 0 ; y--)
                    for (int x = 0; x < tiles_x; x++)
                    {
                        var pixels = baseTexture.GetPixels(sourceWidth * x, sourceHeight * y, sourceWidth, sourceHeight);
                        float totalAlpha = 0;
                        foreach (var pixel in pixels)
                        {
                            totalAlpha += pixel.a;
                        }
                        if (totalAlpha < 1) //it's empty, bro.
                            continue;
                        var tempTex = new Texture2D(sourceWidth, sourceHeight, TextureFormat.ARGB32, false);
                        tempTex.SetPixels(pixels);
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
                        TextureScale.Bilinear(tempTex, potWidth, potHeight);
                        tempList.Add(tempTex);
                    }
                texArray = new Texture2DArray(potWidth, potHeight, tempList.Count, TextureFormat.ARGB32, mipmaps);
                for(int i = 0; i < tempList.Count; i++)
                {
                    texArray.SetPixels(tempList[i].GetPixels(), i);
                }
                texArray.Apply();
                texArray.name = baseTexture.name + "Array";
                tempList.Clear();
                Resources.UnloadUnusedAssets();
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
            if(GUILayout.Button("Generate Bump"))
            {
                normalArray = new Texture2DArray(texArray.width, texArray.height, texArray.depth, TextureFormat.ARGB32, true, true);
                for(int i = 0; i < normalArray.depth; i++)
                {
                    normalArray.SetPixels(TextureTools.Bevel(texArray.GetPixels(i), texArray.width, texArray.height), i);
                }
                normalArray.Apply();
            }
        }
        if (normalArray != null)
        {
            previewIndex = EditorGUILayout.IntSlider(previewIndex, 0, normalArray.depth - 1);
            if (normalPreviewTexture == null)
                normalPreviewTexture = new Texture2D(normalArray.width, normalArray.height, TextureFormat.ARGB32, false);
            if (normalPreviewTexture.width != normalArray.width || normalPreviewTexture.height != normalArray.height)
                normalPreviewTexture.Resize(normalArray.width, normalArray.height);

            var previewPixels = normalArray.GetPixels(previewIndex);

            for(int i = 0; i < previewPixels.Length; i++)
            {
                Color color = previewPixels[i];
                previewPixels[i] = new Color(color.a, color.g, Mathf.Sqrt(1 - (color.a * 2 - 1) * (color.a * 2 - 1) - (color.g * 2 - 1) * (color.g * 2 - 1)));
            }

            normalPreviewTexture.SetPixels(previewPixels);
            normalPreviewTexture.Apply();

            EditorGUI.DrawPreviewTexture(EditorGUILayout.GetControlRect(false, normalPreviewTexture.height), normalPreviewTexture, null, UnityEngine.ScaleMode.ScaleToFit);

            if (GUILayout.Button("Save Texture Array"))
            {
                var path = EditorUtility.SaveFilePanelInProject("Save texture array to asset", normalArray.name + ".asset", "asset", "Please select a filename to save the texture atlas to.");
                AssetDatabase.CreateAsset(normalArray, path);
                AssetDatabase.Refresh();
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
