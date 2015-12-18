using UnityEngine;
using System.Collections;

public class Spammer : MonoBehaviour {
    public Transform source;

    public int numX = 1;
    public int numY = 1;
    public int numZ = 1;

    public float spacing = 1;


	// Use this for initialization
	void Start () {
        for (int x = 0; x < numX; x++)
            for (int y = 0; y < numY; y++)
                for (int z = 0; z < numZ; z++)
                {
                    Transform instance = Instantiate(source);
                    instance.parent = transform;
                    instance.position = new Vector3(x, y, z) * spacing;
                    instance.name = x + ", " + y + ", " + z;
                }
        Debug.Log("Spammed " + numX * numY * numZ + "Instances");
    }
}
