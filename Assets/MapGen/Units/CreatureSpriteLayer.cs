using RemoteFortressReader;
using UnityEngine;

[System.Serializable]
public class CreatureSpriteLayer
{
    public enum SpriteSource
    {
        Static,
        Bodypart,
        Equipment,
    }

    public enum ColorSource
    {
        None,
        Fixed,
        Material,
        BodyPart,
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
    [ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
    public Color color = new Color(0.5f,0.5f,0.5f,0.5f);
    public HairType hairType;
    public HairStyle hairStyle;
    public int hairMin = -1;
    public int hairMax = -1;
    public int patternIndex = 0;
    public bool metal = false;
    public Vector2 positionOffset = Vector2.zero;
}
