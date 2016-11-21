

public class VoxelStencil
{
    protected Voxel.State fillType;
    protected int centerX, centerY, radius;

    public virtual void Initialize(Voxel.State fillType, int radius)
    {
        this.fillType = fillType;
        this.radius = radius;
    }

    public virtual void SetCenter(int x, int y)
    {
        centerX = x;
        centerY = y;
    }

    public int XStart
    {
        get
        {
            return centerX - radius;
        }
    }

    public int XEnd
    {
        get
        {
            return centerX + radius;
        }
    }

    public int YStart
    {
        get
        {
            return centerY - radius;
        }
    }

    public int YEnd
    {
        get
        {
            return centerY + radius;
        }
    }

    public virtual Voxel.State Apply(int x, int y, Voxel.State voxel)
    {
        return fillType;
    }
}