using DFHack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        Instance = this;
    }

    public Mesh waveMesh;
    public Material waveMaterial;
    private Matrix4x4[] positions;
    public Vector3 scale = Vector3.one;

    private void Update()
    {
        DrawWaves();
    }



    private void DrawWaves()
    {
        if (DFConnection.Instance == null)
            return;
        var waves = DFConnection.Instance.waves; // Keep a copy, because the other one might be replaced.

        if (waves == null)
            return;
        if (waves.Count == 0)
            return;

        if (positions == null || positions.Length < waves.Count)
            positions = new Matrix4x4[waves.Count];
        
        for(int i = 0; i < waves.Count; i++)
        {
            var wave = waves[i];
            var dist = ((DFCoord)wave.pos - wave.dest).Max;
            float size = (21 - dist) / 20.0f;
            positions[i] = Matrix4x4.TRS(
                GameMap.DFtoUnityCoord(wave.pos),
                Quaternion.LookRotation(GameMap.DFtoUnityCoord(wave.dest) - GameMap.DFtoUnityCoord(wave.pos)),
                new Vector3(scale.x, size * scale.y, scale.z));
        }

        Graphics.DrawMeshInstanced(waveMesh, 0, waveMaterial, positions, waves.Count);
    }
}
