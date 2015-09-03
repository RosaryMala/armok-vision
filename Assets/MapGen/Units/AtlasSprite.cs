using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class AtlasSprite : MonoBehaviour {

    public float tileDistance = 0.001f;
    public float width = 2.0f;
    public float height = 2.0f;

    public int Count
    {
        get { return vertices.Count / 4; }
    }

    List<Vector3> vertices;
    List<Color> colors;
    List<Vector2> uvs;
    List<int> triangles;

    Mesh mesh;

    MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        vertices = new List<Vector3>();
        colors = new List<Color>();
        uvs = new List<Vector2>();
        triangles = new List<int>();

        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    public void ClearMesh()
    {
        vertices.Clear();
        colors.Clear();
        uvs.Clear();
        triangles.Clear();
        mesh.Clear();
    }

    Rect CharacterToRect(int character)
    {
        int x = character % 16;
        int y = character / 16;
        return new Rect((float)x / 16.0f, (float)y / 16.0f, 1 / 16.0f, 1 / 16.0f);
    }

    public void AddTile(int character = 0, Color color = default(Color))
    {
        int startVertex = vertices.Count;
        Rect rect = CharacterToRect(character);

        float offset = Count * tileDistance;

        vertices.Add(new Vector3(-width / 2, -height / 2, offset));
        vertices.Add(new Vector3(width / 2, -height / 2, offset));
        vertices.Add(new Vector3(-width / 2, height / 2, offset));
        vertices.Add(new Vector3(width / 2, height / 2, offset));

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        uvs.Add(new Vector2(rect.xMin, 1 - rect.yMax));
        uvs.Add(new Vector2(rect.xMax, 1 - rect.yMax));
        uvs.Add(new Vector2(rect.xMin, 1 - rect.yMin));
        uvs.Add(new Vector2(rect.xMax, 1 - rect.yMin));

        triangles.Add(startVertex + 0);
        triangles.Add(startVertex + 2);
        triangles.Add(startVertex + 1);

        triangles.Add(startVertex + 1);
        triangles.Add(startVertex + 2);
        triangles.Add(startVertex + 3);

        mesh.vertices = vertices.ToArray();
        mesh.colors = colors.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
    }

}
