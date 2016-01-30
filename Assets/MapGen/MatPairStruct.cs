using RemoteFortressReader;

public struct MatPairStruct
{
    public readonly int mat_index;
    public readonly int mat_type;

    public int Type { get { return mat_type; } }
    public int SubType { get { return mat_index; } }

    public static implicit operator MatPairStruct(MatPair input)
    {
        if (input == null)
            return new MatPairStruct(-1, -1);
        return new MatPairStruct(input.mat_type, input.mat_index);
    }
    public static implicit operator MatPair(MatPairStruct input)
    {
        MatPair output = new MatPair();
        output.mat_index = input.mat_index;
        output.mat_type = input.mat_type;
        return output;

    }

    public MatPairStruct(int type, int index)
    {
        mat_index = index;
        mat_type = type;
    }

    public override string ToString()
    {
        return string.Format("[{0},{1}]", mat_type, mat_index);
    }
}