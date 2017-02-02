public struct TileDef
{
    public readonly int page;
    public readonly int index;
    public readonly bool colored;
    public TileDef(int page, int index, bool colored)
    {
        this.page = page;
        this.index = index;
        this.colored = colored;
    }
}