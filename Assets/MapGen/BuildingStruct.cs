using RemoteFortressReader;

public struct BuildingStruct
{
    public readonly int building_type;
    public readonly int building_subtype;
    public readonly int building_custom;

    public static implicit operator BuildingStruct(BuildingType input)
    {
        return new BuildingStruct(input.BuildingType_, input.BuildingSubtype, input.BuildingCustom);
    }

    public static implicit operator BuildingType(BuildingStruct input)
    {
        BuildingType output = new BuildingType();
        output.BuildingType_ = input.building_type;
        output.BuildingSubtype = input.building_subtype;
        output.BuildingCustom = input.building_custom;
        return output;
    }

    public BuildingStruct(int type, int subtype, int custom)
    {
        building_type = type;
        building_subtype = subtype;
        building_custom = custom;
    }

    public static bool operator ==(BuildingStruct a, BuildingStruct b)
    {
        return (a.building_type == b.building_type)
            && (a.building_subtype == b.building_subtype)
            && (a.building_custom == b.building_custom);
    }
    public static bool operator !=(BuildingStruct a, BuildingStruct b)
    {
        return (a.building_type != b.building_type)
            || (a.building_subtype != b.building_subtype)
            || (a.building_custom != b.building_custom);
    }
    public override bool Equals(object obj)
    {
        return obj is BuildingStruct && this == (BuildingStruct)obj;
    }
    public override int GetHashCode()
    {
        return building_type + (building_subtype * 200) + (building_subtype * 20000);
    }

    public override string ToString()
    {
        return "[" + building_type + "," + building_subtype + "," + building_custom + "]";
    }
}
