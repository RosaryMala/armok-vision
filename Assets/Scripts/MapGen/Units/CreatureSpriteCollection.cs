using DF.Enums;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CreatureSpriteCollection : ScriptableObject
{
    public enum Special
    {
        Normal,
        Military,
        Ghost
    }

    public string race;
    public string caste;
    public List<CreatureSpriteLayer> spriteLayers;
    public Special special;
    public profession profession;
    public float standardSize;
    public float standardArea;
    public float standardLength;

    void OnValidate()
    {
        standardLength = Mathf.Pow(standardSize, 1.0f / 3.0f) * 10;
        standardArea = Mathf.Pow(standardSize, 2.0f / 3.0f);
    }
}
