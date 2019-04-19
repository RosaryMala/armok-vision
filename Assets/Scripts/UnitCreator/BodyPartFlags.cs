using System.Collections.Generic;

[System.Serializable]
public class BodyPartFlags
{
    public enum BodyPartRawFlags
    {
        HEAD,
        UPPERBODY,
        LOWERBODY,
        SIGHT,
        EMBEDDED,
        INTERNAL,
        CIRCULATION,
        SKELETON,
        LIMB,
        GRASP,
        STANCE,
        GUTS,
        BREATHE,
        SMALL,
        THROAT,
        JOINT,
        THOUGHT,
        NERVOUS,
        RIGHT,
        LEFT,
        HEAR,
        SMELL,
        FLIER,
        DIGIT,
        MOUTH,
        APERTURE,
        SOCKET,
        TOTEMABLE,
        anon_1,
        anon_2,
        UNDER_PRESSURE,
        anon_3,
        VERMIN_BUTCHER_ITEM,
        CONNECTOR,
        anon_4,
        anon_5,
        anon_6,
        anon_7,
        GELDABLE
    };
    public bool head;
    public bool upperbody;
    public bool lowerbody;
    public bool sight;
    public bool embedded;
    public bool _internal;
    public bool circulation;
    public bool skeleton;
    public bool limb;
    public bool grasp;
    public bool stance;
    public bool guts;
    public bool breathe;
    public bool small;
    public bool throat;
    public bool joint;
    public bool thought;
    public bool nervous;
    public bool right;
    public bool left;
    public bool hear;
    public bool smell;
    public bool flier;
    public bool digit;
    public bool mouth;
    public bool aperture;
    public bool socket;
    public bool totemable;
    public bool anon_1;
    public bool anon_2;
    public bool under_pressure;
    public bool anon_3;
    public bool vermin_butcher_item;
    public bool connector;
    public bool anon_4;
    public bool anon_5;
    public bool anon_6;
    public bool anon_7;
    public bool geldable;

    public BodyPartFlags(List<bool> flagList)
    {
        head = flagList[0];
        upperbody = flagList[1];
        lowerbody = flagList[2];
        sight = flagList[3];
        embedded = flagList[4];
        _internal = flagList[5];
        circulation = flagList[6];
        skeleton = flagList[7];
        limb = flagList[8];
        grasp = flagList[9];
        stance = flagList[10];
        guts = flagList[11];
        breathe = flagList[12];
        small = flagList[13];
        throat = flagList[14];
        joint = flagList[15];
        thought = flagList[16];
        nervous = flagList[17];
        right = flagList[18];
        left = flagList[19];
        hear = flagList[20];
        smell = flagList[21];
        flier = flagList[22];
        digit = flagList[23];
        mouth = flagList[24];
        aperture = flagList[25];
        socket = flagList[26];
        totemable = flagList[27];
        anon_1 = flagList[28];
        anon_2 = flagList[29];
        under_pressure = flagList[30];
        anon_3 = flagList[31];
        vermin_butcher_item = flagList[32];
        connector = flagList[33];
        anon_4 = flagList[34];
        anon_5 = flagList[35];
        anon_6 = flagList[36];
        anon_7 = flagList[37];
        geldable = flagList[38];
    }
}
