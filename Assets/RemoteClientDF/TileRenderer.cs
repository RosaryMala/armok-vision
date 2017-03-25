using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TileRenderer : MonoBehaviour
{

    public int width;
    public int height;
    public byte[] tiles;
    public byte[] fgColors;
    public byte[] bgColors;

    public float tilewidth;
    public float tileheight;

    Vector3[] vertices;
    Color[] colors;
    Vector2[] uvs;
    int[] triangles;

    Mesh mesh;

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        mesh = new Mesh();
        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
    }


    // Use this for initialization
    void Start()
    {
        CaptureScreen();
    }

    // Update is called once per frame
    void Update()
    {
        CaptureScreen();
    }

    void CaptureScreen()
    {
        if (DFConnection.Instance == null)
            return;
        var capture = DFConnection.Instance.PopScreenUpdate();
        if (capture == null)
            return;

        if (width != capture.width || height != capture.height)
        {
            width = Mathf.Min((int)capture.width, 16383 / (int)capture.height);
            height = (int)capture.height;
            GenerateTiles();
        }

        for (int i = 0; i < width * height; i++)
        {
            var tile = capture.tiles[i];

            tiles[i] = (byte)tile.character;
            fgColors[i] = (byte)tile.foreground;
            bgColors[i] = (byte)tile.background;
        }

        UpdateTiles();
    }

    int coord2index(int x, int y)
    {
        return x * height + y;
    }

    void GenerateTiles()
    {
        if (width == 0 || height == 0)
        {
            tiles = null;
            vertices = null;
            colors = null;
            uvs = null;
        }
        tiles = new byte[width * height];
        fgColors = new byte[width * height];
        bgColors = new byte[width * height];
        vertices = new Vector3[width * height * 4];
        colors = new Color[width * height * 4];
        uvs = new Vector2[width * height * 4];
        triangles = new int[width * height * 6];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = coord2index(x, height - y - 1) * 4;

                int fakeX = x - (width / 2);
                int fakeY = y;// - (height / 2);

                vertices[index] = new Vector3(fakeX * tilewidth, fakeY * tileheight, 0);
                vertices[index + 1] = new Vector3((fakeX + 1) * tilewidth, fakeY * tileheight, 0);
                vertices[index + 2] = new Vector3(fakeX * tilewidth, (fakeY + 1) * tileheight, 0);
                vertices[index + 3] = new Vector3((fakeX + 1) * tilewidth, (fakeY + 1) * tileheight, 0);

                int triIndex = coord2index(x, y) * 6;

                triangles[triIndex] = index;
                triangles[triIndex + 1] = index + 2;
                triangles[triIndex + 2] = index + 1;

                triangles[triIndex + 3] = index + 1;
                triangles[triIndex + 4] = index + 2;
                triangles[triIndex + 5] = index + 3;
            }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    void UpdateTiles()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = coord2index(x, y);

                byte tile = tiles[index];

                int uvx = tile % 16;
                int uvy = 15 - (tile / 16);

                uvs[index * 4] = new Vector2(uvx / 16.0f, uvy / 16.0f);
                uvs[(index * 4) + 1] = new Vector2((uvx + 1) / 16.0f, uvy / 16.0f);
                uvs[(index * 4) + 2] = new Vector2(uvx / 16.0f, (uvy + 1) / 16.0f);
                uvs[(index * 4) + 3] = new Vector2((uvx + 1) / 16.0f, (uvy + 1) / 16.0f);

                Color color = new Color((fgColors[index] + 0.5f) / 16.0f, (bgColors[index] + 0.5f) / 16.0f, 0.5f);

                colors[index * 4] = color;
                colors[(index * 4) + 1] = color;
                colors[(index * 4) + 2] = color;
                colors[(index * 4) + 3] = color;
            }
        mesh.uv = uvs;
        mesh.colors = colors;
    }
}
