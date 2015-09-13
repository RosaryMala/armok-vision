using RemoteFortressReader;

public struct MatPairStruct
{
    public int mat_index;
    public int mat_type;

    public static implicit operator MatPairStruct(MatPair input)
    {
        MatPairStruct output;
        output.mat_index = input.mat_index;
        output.mat_type = input.mat_type;
        return output;
    }
    public static implicit operator MatPair(MatPairStruct input)
    {
        MatPair output = new MatPair();
        output.mat_index = input.mat_index;
        output.mat_type = input.mat_type;
        return output;

    }

    public MatPairStruct(int index, int type)
    {
        mat_index = index;
        mat_type = type;
    }
}