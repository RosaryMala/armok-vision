using RemoteFortressReader;
using System;

public struct MatPairStruct : IComparable
{
    public readonly int mat_index;
    public readonly int mat_type;

    public DF.Enums.ItemType Type { get { return (DF.Enums.ItemType)mat_type; } }
    public int SubType { get { return mat_index; } }

    public int Race { get { return mat_type; } }
    public int Caste { get { return mat_index; } }

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

    public static bool operator !=(MatPairStruct a, MatPairStruct b)
    {
        return a.mat_index != b.mat_index || a.mat_type != b.mat_type;
    }
    public static bool operator ==(MatPairStruct a, MatPairStruct b)
    {
        return a.mat_index == b.mat_index && a.mat_type == b.mat_type;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is MatPairStruct))
            return false;
        return this == (MatPairStruct)obj;
    }

    public override int GetHashCode()
    {
        return mat_index * 65535 + mat_type;
    }

    public MatPairStruct(int type, int index)
    {
        mat_index = index;
        mat_type = type;
    }

    public MatPairStruct(DF.Enums.ItemType type, int index)
    {
        mat_index = index;
        mat_type = (int)type;
    }


    public override string ToString()
    {
        return string.Format("[{0},{1}]", mat_type, mat_index);
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        if (!(obj is MatPairStruct)) return 1;
        var b = (MatPairStruct)obj;
        if (mat_type == b.mat_type)
            return mat_index.CompareTo(b.mat_index);
        else
            return mat_type.CompareTo(b.mat_type);
    }
}