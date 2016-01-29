using RemoteFortressReader;

public struct IntPair
{
    public readonly int mat_index;
    public readonly int mat_type;

    public int Type { get { return mat_type; } }
    public int SubType { get { return mat_index; } }

    public static implicit operator IntPair(MatPair input)
    {
        return new IntPair(input.mat_type, input.mat_index);
    }
    public static implicit operator MatPair(IntPair input)
    {
        MatPair output = new MatPair();
        output.mat_index = input.mat_index;
        output.mat_type = input.mat_type;
        return output;

    }

    public IntPair(int type, int index)
    {
        mat_index = index;
        mat_type = type;
    }

    public override string ToString()
    {
        return string.Format("[{0},{1}]", mat_type, mat_index);
    }
}