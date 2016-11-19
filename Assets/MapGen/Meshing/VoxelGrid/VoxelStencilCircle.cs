


public class VoxelStencilCircle : VoxelStencil
{

    private int sqrRadius;

    public override void Initialize(bool fillType, int radius)
    {
        base.Initialize(fillType, radius);
        sqrRadius = radius * radius;
    }

    public override bool Apply(int x, int y, bool voxel)
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