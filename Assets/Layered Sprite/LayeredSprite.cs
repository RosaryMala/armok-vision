using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayeredSprite : MonoBehaviour
{
    public Texture2D sprite_sheet;
    public int sprite_count_x = 1;
    public int sprite_count_y = 1;
    public int virtual_sprite_wrap = 20;

    public float desired_height = 1.0f;

    public bool Do_Sprite = false;

    int sprite_layer_count = 0;
    public List<int> sprite_layer_indices;
    public List<Color> sprite_layer_colors;
    public List<bool> sprite_layer_enabled;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnValidate()
    {
        EqualizeListLengths();
        if(Do_Sprite)
        {
            UpdateQuad();
            renderer.material.mainTexture = CompileSprite();
            Resources.UnloadUnusedAssets();
            Do_Sprite = false;
        }
    }

    void OnDestroy()
    {
        DestroyImmediate(renderer.material);
    }

    public static bool nearlyEqual(float a, float b, float epsilon) {
        float absA = Mathf.Abs(a);
        float absB = Mathf.Abs(b);
        float diff = Mathf.Abs(a - b);

        if (a == b) { // shortcut, handles infinities
            return true;
        } else if (a == 0 || b == 0 || diff < float.MinValue) {
            // a or b is zero or both are extremely close to it
            // relative error is less meaningful here
            return diff < (epsilon * float.MinValue);
        } else { // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }

    bool EqualizeListLengths()
    {
        //Go through each layer property and make sure it's the same size as the one that's changed.
        if (sprite_layer_indices.Count > sprite_layer_count)
        {
            sprite_layer_colors.AddRange(Enumerable.Repeat<Color>(Color.white, sprite_layer_indices.Count - sprite_layer_count));
            sprite_layer_enabled.AddRange(Enumerable.Repeat<bool>(false, sprite_layer_indices.Count - sprite_layer_count));
            sprite_layer_count = sprite_layer_indices.Count;
        }
        else if (sprite_layer_colors.Count > sprite_layer_count)
        {
            sprite_layer_indices.AddRange(Enumerable.Repeat<int>(0, sprite_layer_colors.Count - sprite_layer_count));
            sprite_layer_enabled.AddRange(Enumerable.Repeat<bool>(false, sprite_layer_colors.Count - sprite_layer_count));
            sprite_layer_count = sprite_layer_colors.Count;
        }
        else if (sprite_layer_enabled.Count > sprite_layer_count)
        {
            sprite_layer_indices.AddRange(Enumerable.Repeat<int>(0, sprite_layer_enabled.Count - sprite_layer_count));
            sprite_layer_colors.AddRange(Enumerable.Repeat<Color>(Color.white, sprite_layer_enabled.Count - sprite_layer_count));
            sprite_layer_count = sprite_layer_enabled.Count;
        }
        else if (sprite_layer_indices.Count < sprite_layer_count)
        {
            sprite_layer_colors.RemoveRange(sprite_layer_indices.Count, sprite_layer_colors.Count - sprite_layer_indices.Count);
            sprite_layer_enabled.RemoveRange(sprite_layer_indices.Count, sprite_layer_enabled.Count - sprite_layer_indices.Count);
            sprite_layer_count = sprite_layer_indices.Count;
        }
        else if (sprite_layer_colors.Count < sprite_layer_count)
        {
            sprite_layer_indices.RemoveRange(sprite_layer_colors.Count, sprite_layer_indices.Count - sprite_layer_colors.Count);
            sprite_layer_enabled.RemoveRange(sprite_layer_colors.Count, sprite_layer_enabled.Count - sprite_layer_colors.Count);
            sprite_layer_count = sprite_layer_colors.Count;
        }
        else if (sprite_layer_enabled.Count < sprite_layer_count)
        {
            sprite_layer_indices.RemoveRange(sprite_layer_enabled.Count, sprite_layer_indices.Count - sprite_layer_enabled.Count);
            sprite_layer_colors.RemoveRange(sprite_layer_enabled.Count, sprite_layer_colors.Count - sprite_layer_enabled.Count);
            sprite_layer_count = sprite_layer_enabled.Count;
        }
        else return false;
        return true;
    }

    void UpdateQuad()
    {
        float desired_ratio = 1.0f;
        if (sprite_sheet != null)
        {
            desired_ratio = (float)(sprite_sheet.height / sprite_count_y) / (float)(sprite_sheet.width / sprite_count_x);
        }
        if (float.IsInfinity(desired_ratio))
            desired_ratio = 1.0f;
        MeshFilter mf = GetComponent<MeshFilter>();
        DestroyImmediate(mf.sharedMesh);
        mf.sharedMesh = MeshUtils.MakeQuad(desired_height, desired_ratio);
    }

    Texture2D CompileSprite()
    {
        var s1 = System.Diagnostics.Stopwatch.StartNew();
        int sprite_width = sprite_sheet.width / sprite_count_x;
        int sprite_height = sprite_sheet.height / sprite_count_y;
        Texture2D base_sprite = new Texture2D(sprite_width, sprite_height);
        int active_layers = 0;
        for (int yy = 0; yy < sprite_height; yy++)
            for (int xx = 0; xx < sprite_width; xx++)
            {
                    base_sprite.SetPixel(xx, yy, Color.clear);
            }
        for (int i = 0; i < sprite_layer_count; i++)
        {
            if (!sprite_layer_enabled[i])
                continue;
            int origin_x = (sprite_layer_indices[i] % virtual_sprite_wrap) * sprite_width;
            int origin_y = sprite_sheet.height - ((sprite_layer_indices[i] / virtual_sprite_wrap) * sprite_height) - sprite_height;
            for(int yy = 0; yy < sprite_height; yy++)
                for(int xx = 0; xx < sprite_width; xx++)
                {
                    //we're not doing alpha blending for now.
                    Color brush_color = sprite_sheet.GetPixel(xx + origin_x, yy + origin_y);
                    if (brush_color.a >= 0.5)
                    {
                        brush_color *= sprite_layer_colors[i];
                        brush_color.a = 1.0f;
                        base_sprite.SetPixel(xx, yy, brush_color);
                    }
                }
            active_layers++;
        }
        base_sprite.Apply();
        s1.Stop();
        Debug.Log("Regenerating a " + sprite_width + "x" + sprite_height + "sprite with " + active_layers + " active layers took " + s1.ElapsedMilliseconds + "ms.");
        return base_sprite;
    }
}
