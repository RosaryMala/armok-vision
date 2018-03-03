using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    private Material[] materials;
    public Texture2D defaultDiffuse;
    public Texture2D defaultSpecular;
    Dictionary<string, string> diffuseSpecularTextures = new Dictionary<string, string>();

    // Use this for initialization
    void Start()
    {
        diffuseSpecularTextures[defaultDiffuse.GetInstanceID() + ":" + defaultSpecular.GetInstanceID()] = defaultDiffuse.name + ":" + defaultSpecular.name;
        materials = Resources.LoadAll<Material>("Materials");
        Debug.Log("Loaded " + materials.Length + " materials");
        foreach (var mat in materials)
        {
            Texture2D diffTex = mat.GetTexture("_MainTex") as Texture2D;
            Texture2D specTex = mat.GetTexture("_Specular") as Texture2D;
            string id;
            string name;
            if (diffTex == null)
            {
                id = defaultDiffuse.GetInstanceID().ToString() + ":";
                name = defaultDiffuse.name + ":";
            }
            else
            {
                id = diffTex.GetInstanceID().ToString() + ":";
                name = diffTex.name + ":";
            }

            if (specTex == null)
            {
                id += defaultSpecular.GetInstanceID().ToString();
                name += defaultSpecular.name;
            }
            else
            {
                id += specTex.GetInstanceID().ToString();
                name += specTex.name;
            }
            diffuseSpecularTextures[id] = name;
            Debug.Log(mat.name + ", " + name);

        }
        Debug.Log("Found " + diffuseSpecularTextures.Count + " unique diffuse/specular texture combos");
        foreach (var item in diffuseSpecularTextures)
        {
            Debug.Log(item.Value);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}
