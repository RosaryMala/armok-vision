using UnityEngine;
using System;

[Serializable]
public class Voxel
{
    public enum State
    {
        Empty,
        Filled,
        Intruded
    }

    public State state;
    public bool edge;

    public Vector3 position, eastEdge, southEdge, cornerPosition;

    public Voxel(int x, int y, float size)
    {
        position.x = (x + 0.5f) * size;
        position.z = (-y - 0.5f) * size;

        eastEdge = position;
        eastEdge.x += size * 0.5f;
        southEdge = position;
        southEdge.z -= size * 0.5f;
        cornerPosition = position;
        cornerPosition.x += size * 0.5f;
        cornerPosition.z -= size * 0.5f;
    }

    public Voxel() { }

    public void BecomeXDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        eastEdge = voxel.eastEdge;
        southEdge = voxel.southEdge;
        cornerPosition = voxel.cornerPosition;
        position.x += offset;
        eastEdge.x += offset;
        southEdge.x += offset;
        cornerPosition.x += offset;
    }
    public void BecomeYDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        eastEdge = voxel.eastEdge;
        southEdge = voxel.southEdge;
        cornerPosition = voxel.cornerPosition;
        position.z -= offset;
        eastEdge.z -= offset;
        southEdge.z -= offset;
        cornerPosition.z -= offset;
    }
    public void BecomeXYDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        eastEdge = voxel.eastEdge;
        southEdge = voxel.southEdge;
        cornerPosition = voxel.cornerPosition;
        position.x += offset;
        position.z -= offset;
        eastEdge.x += offset;
        eastEdge.z -= offset;
        southEdge.x += offset;
        southEdge.z -= offset;
        cornerPosition.x += offset;
        cornerPosition.z -= offset;
    }
}