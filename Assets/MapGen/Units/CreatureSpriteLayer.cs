using RemoteFortressReader;
using UnityEngine;

[System.Serializable]
public class CreatureSpriteLayer
{
    public enum SpriteSource
    {
        Static,
        Bodypart,
        Equipment
    }

    public enum ColorSource
    {
        None,
        Fixed,
        Material,
        Job
    }

    public enum HairType
    {
        None,
        Hair,
        Beard,
        Moustache,
        Sideburns
    }

    public SpriteSource spriteSource;
    public ColorSource colorSource;
    public string token;
    public Sprite spriteTexture;
    public bool preview;
    public Color color = new Color(0.5f,0.5f,0.5f,0.5f);
    public HairType hairType;
    public HairStyle hairStyle;
}
