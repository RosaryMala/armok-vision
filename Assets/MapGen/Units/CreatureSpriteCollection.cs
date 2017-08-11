using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CreatureSpriteCollection : ScriptableObject
{
    public string race;
    public string caste;
    public List<CreatureSpriteLayer> spriteLayers;
}
