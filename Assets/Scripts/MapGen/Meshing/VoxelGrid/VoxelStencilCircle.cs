


public class VoxelStencilCircle : VoxelStencil
{

    private int sqrRadius;

    public override void Initialize(Voxel.State fillType, int radius)
    {
        base.Initialize(fillType, radius);
        sqrRadius = radius * radius;
    }

    public override Voxel.State Apply(int x, int y, Voxel.State voxel)
    {
        x -= centerX;
        y -= centerY;
        if (x * x + y * y <= sqrRadius)
        {
            return fillType;
        }
        return voxel;
    }
}