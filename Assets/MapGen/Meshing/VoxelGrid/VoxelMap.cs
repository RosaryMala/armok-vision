using System;
using UnityEngine;

public class VoxelMap : MonoBehaviour
{
    public float size = 2f;

    public int voxelResolution = 8;
    public int chunkResolution = 2;

    public VoxelGrid voxelGridPrefab;

    private VoxelGrid[] chunks;

    private float chunkSize, voxelSize, halfSize;

    private void Awake()
    {
        halfSize = size * 0.5f;
        chunkSize = size / chunkResolution;
        voxelSize = chunkSize / voxelResolution;

        chunks = new VoxelGrid[chunkResolution * chunkResolution];
        for (int i = 0, y = 0; y < chunkResolution; y++)
        {
            for (int x = 0; x < chunkResolution; x++, i++)
            {
                CreateChunk(i, x, y);
            }
        }
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.size = new Vector3(size, 0, size);
    }

    private void CreateChunk(int i, int x, int y)
    {
        VoxelGrid chunk = Instantiate(voxelGridPrefab) as VoxelGrid;
        chunk.Initialize(voxelResolution, chunkSize);
        chunk.transform.parent = transform;
        chunk.transform.localPosition = new Vector3(x * chunkSize - halfSize, 0, -y * chunkSize + halfSize);
        chunk.cornerType = (VoxelGrid.CornerType)cornerTypeIndex;
        chunks[i] = chunk;
        if (x > 0)
        {
            chunks[i - 1].xNeighbor = chunk;
        }
        if (y > 0)
        {
            chunks[i - chunkResolution].yNeighbor = chunk;
            if (x > 0)
            {
                chunks[i - chunkResolution - 1].xyNeighbor = chunk;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (hitInfo.collider.gameObject == gameObject)
                {
                    EditVoxels(transform.InverseTransformPoint(hitInfo.point));
                }
            }
        }
    }

    private void EditVoxels(Vector3 point)
    {
        int centerX = (int)((point.x + halfSize) / voxelSize);
        int centerY = (int)((-point.z + halfSize) / voxelSize);

        int xStart = (centerX - radiusIndex - 1) / voxelResolution;
        if (xStart < 0)
        {
            xStart = 0;
        }
        int xEnd = (centerX + radiusIndex) / voxelResolution;
        if (xEnd >= chunkResolution)
        {
            xEnd = chunkResolution - 1;
        }
        int yStart = (centerY - radiusIndex - 1) / voxelResolution;
        if (yStart < 0)
        {
            yStart = 0;
        }
        int yEnd = (centerY + radiusIndex) / voxelResolution;
        if (yEnd >= chunkResolution)
        {
            yEnd = chunkResolution - 1;
        }

        VoxelStencil activeStencil = stencils[stencilIndex];
        activeStencil.Initialize((Voxel.State)fillTypeIndex, radiusIndex);

        int voxelYOffset = yEnd * voxelResolution;
        for (int y = yEnd; y >= yStart; y--)
        {
            int i = y * chunkResolution + xEnd;
            int voxelXOffset = xEnd * voxelResolution;
            for (int x = xEnd; x >= xStart; x--, i--)
            {
                activeStencil.SetCenter(centerX - voxelXOffset, centerY - voxelYOffset);
                chunks[i].Apply(activeStencil);
                voxelXOffset -= voxelResolution;
            }
            voxelYOffset -= voxelResolution;
        }
    }

    private static string[] fillTypeNames = { "Empty", "Filled", "Intruded" };

    private static string[] radiusNames = { "0", "1", "2", "3", "4", "5" };

    private static string[] stencilNames = { "Square", "Circle" };

    private static string[] cornerType = { "Diamond", "Square", "Rounded" };

    private static string[] gapType = { "Opened", "Normal" };

    private static string[] crossingType = { "Open ", "Closed" };

    private int fillTypeIndex = 1;
    private int radiusIndex, stencilIndex;
    private int cornerTypeIndex;
    private int gapTypeIndex;
    private int crossingTypeIndex;

    private VoxelStencil[] stencils = {
        new VoxelStencil(),
        new VoxelStencilCircle()
    };

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(4f, 4f, 150f, 500f));
        GUILayout.Label("Fill Type");
        fillTypeIndex = GUILayout.SelectionGrid(fillTypeIndex, fillTypeNames, 3);
        GUILayout.Label("Radius");
        radiusIndex = GUILayout.SelectionGrid(radiusIndex, radiusNames, 6);
        GUILayout.Label("Stencil");
        stencilIndex = GUILayout.SelectionGrid(stencilIndex, stencilNames, 2);
        GUILayout.Label("Corner Type");
        int corner = GUILayout.SelectionGrid(cornerTypeIndex, cornerType, 3);
        if(cornerTypeIndex != corner)
        {
            cornerTypeIndex = corner;
            foreach (var item in chunks)
            {
                item.cornerType = (VoxelGrid.CornerType)cornerTypeIndex;
            }
        }
        GUILayout.Label("Diagonal Gaps");
        int gap = GUILayout.SelectionGrid(gapTypeIndex, gapType, 2);
        if(gapTypeIndex != gap)
        {
            gapTypeIndex = gap;
            foreach (var item in chunks)
            {
                item.filledGaps = gapTypeIndex == 1;
            }

        }
        GUILayout.Label("Diagonal Type");
        int crossing = GUILayout.SelectionGrid(crossingTypeIndex, crossingType, 2);
        if(crossingTypeIndex != crossing)
        {
            crossingTypeIndex = crossing;
            foreach (var item in chunks)
            {
                item.saddleCrossing = crossingTypeIndex == 1;
            }
        }
        if (GUILayout.Button("Invert"))
        {
            foreach (var item in chunks)
            {
                item.Invert();
            }
        }
        GUILayout.EndArea();
    }
}