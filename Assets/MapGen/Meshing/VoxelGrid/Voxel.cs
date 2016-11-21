using UnityEngine;
using System;

[Serializable]
public class Voxel
{

    public bool state;

    public Vector3 position, xEdgePosition, yEdgePosition, cornerPosition;

    public Voxel(int x, int y, float size)
    {
        position.x = (x + 0.5f) * size;
        position.z = (-y - 0.5f) * size;

        xEdgePosition = position;
        xEdgePosition.x += size * 0.5f;
        yEdgePosition = position;
        yEdgePosition.z -= size * 0.5f;
        cornerPosition = position;
        cornerPosition.x += size * 0.5f;
        cornerPosition.z -= size * 0.5f;
    }

    public Voxel() { }

    public void BecomeXDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdgePosition = voxel.xEdgePosition;
        yEdgePosition = voxel.yEdgePosition;
        cornerPosition = voxel.cornerPosition;
        position.x += offset;
        xEdgePosition.x += offset;
        yEdgePosition.x += offset;
        cornerPosition.x += offset;
    }
    public void BecomeYDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdgePosition = voxel.xEdgePosition;
        yEdgePosition = voxel.yEdgePosition;
        cornerPosition = voxel.cornerPosition;
        position.z -= offset;
        xEdgePosition.z -= offset;
        yEdgePosition.z -= offset;
        cornerPosition.z -= offset;
    }
    public void BecomeXYDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdgePosition = voxel.xEdgePosition;
        yEdgePosition = voxel.yEdgePosition;
        cornerPosition = voxel.cornerPosition;
        position.x += offset;
        position.z -= offset;
        xEdgePosition.x += offset;
        xEdgePosition.z -= offset;
        yEdgePosition.x += offset;
        yEdgePosition.z -= offset;
        cornerPosition.x += offset;
        cornerPosition.z -= offset;
    }
}