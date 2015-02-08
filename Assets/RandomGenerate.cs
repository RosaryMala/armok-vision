using UnityEngine;
using System.Collections;

public class RandomGenerate : MonoBehaviour {
    public TerrainData terr;
    public double scale = 1;
    public double verticalScale = 1;
    public double temporalScale = 1;
	// Use this for initialization
	void Start ()
    {
        double min = 0;
        double max = 0;
        OpenSimplexNoise noise = new OpenSimplexNoise();
        for (int xx = 0; xx < 1000; xx++)
            for (int yy = 0; yy < 1000; yy++)
                for (int zz = 0; zz < 1000; zz++)
                {
                    double no = noise.eval(xx, yy, zz);
                    if (no < min)
                        min = no;
                    if (no > max)
                        max = no;
                }
        Debug.Log(min + " - " + max);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
