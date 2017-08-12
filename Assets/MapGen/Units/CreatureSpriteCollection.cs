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
    public string profession;
}
