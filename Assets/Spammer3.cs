using System.Collections.Generic;
using UnityEngine;
public class Spammer3 : MonoBehaviour
{
    ParticleSystem partSys;

    public float size;

    public int number;

    void Awake()
    {
        partSys = GetComponent<ParticleSystem>();
    }

    List<ParticleSystem.Particle> partList = new List<ParticleSystem.Particle>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int x = (int)Mathf.Pow(number, 1.0f / 3.0f);
        int y = (int)Mathf.Pow(number / x, 1.0f / 2.0f);
        int z = number / (x * y);
        partList.Capacity = x * y * z;
        partList.Clear();
        Random.InitState(0);
        for (int xx = 0; xx < x; xx++)
            for (int yy = 0; yy < y; yy++)
                for (int zz = 0; zz < z; zz++)
                {
                    ParticleSystem.Particle part = new ParticleSystem.Particle();
                    part.startColor = Random.ColorHSV();
                    part.position = new Vector3(
                            (xx - x / 2.0f) / x * size,
                            (yy - y / 2.0f) / y * size,
                            (zz - z / 2.0f) / z * size);
                    part.startSize = 1;
                    part.rotation = 0;
                    partList.Add(part);
                }
        partSys.SetParticles(partList.ToArray(), partList.Count);
    }
}
