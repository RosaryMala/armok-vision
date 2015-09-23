using RemoteFortressReader;

public struct BuildingStruct {
    public int building_type;
    public int building_subtype;
    public int building_custom;

    public static implicit operator BuildingStruct(BuildingType input)
    {
        BuildingStruct output;
        output.building_type = input.building_type;
        output.building_subtype = input.building_subtype;
        output.building_custom = input.building_custom;
        return output;
    }

    public static implicit operator BuildingType(BuildingStruct input)
    {
        BuildingType output = new BuildingType();
        output.building_type = input.building_type;
        output.building_subtype = input.building_subtype;
        output.building_custom = input.building_custom;
        return output;
    }

    public BuildingStruct(int type, int subtype, int custom)
    {
        building_type = type;
        building_subtype = subtype;
        building_custom = custom;
    }
}
