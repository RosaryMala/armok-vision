using UnityEngine;
public class Spammer2 : MonoBehaviour
{

    public Mesh mesh;

    public Material material;

    public float size;

    public int number;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int x = (int)Mathf.Pow(number, 1.0f / 3.0f);
        int y = (int)Mathf.Pow(number/x, 1.0f / 2.0f);
        int z = number / (x * y);
        for(int xx = 0; xx < x; xx++)
            for(int yy = 0; yy < y; yy++)
                for (int zz = 0; zz < z; zz++)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS
                        (new Vector3(
                            (xx - x / 2.0f) / x * size,
                            (yy - y / 2.0f) / y * size,
                            (zz - z / 2.0f) / z * size),
                        Quaternion.identity,
                        Vector3.one);
                    Graphics.DrawMesh(mesh, matrix, material, 0);
                }
    }
}
