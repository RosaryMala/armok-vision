using DFHack;
using RemoteFortressReader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    Dictionary<DFCoord, List<FlowInfo>> flowMap = new Dictionary<DFCoord, List<FlowInfo>>();
    private bool dirty;

    Dictionary<FlowType, List<FlowInfo>> flowTypes = new Dictionary<FlowType, List<FlowInfo>>();

    Dictionary<FlowType, ParticleSystem> flowParticles = new Dictionary<FlowType, ParticleSystem>();

    public static FlowManager Instance { get; set; }
    public float dragonColorTempMin = 0;
    public float dragonColorTempMax = 22495;

    public float updateRate = 0.1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        StartCoroutine(DrawParticles());
    }

    public List<FlowInfo> this[DFCoord pos]
    {
        set
        {
            flowMap[pos] = value;
            dirty = true;
        }
    }

    private void Update()
    {
        if(dirty)
        {
            foreach (var item in flowTypes)
            {
                item.Value.Clear();
            }

            foreach (var pos in flowMap)
            {
                foreach (var flow in pos.Value)
                {
                    if (!flowTypes.ContainsKey(flow.type))
                        flowTypes[flow.type] = new List<FlowInfo>();
                    if(flow.density > 0)
                        flowTypes[flow.type].Add(flow);
                }
            }
            dirty = false;
        }
    }

    HashSet<FlowType> warningGiven = new HashSet<FlowType>();

    IEnumerator DrawParticles()
    {
        while (true)
        {
            foreach (var item in flowTypes)
            {
                if (!flowParticles.ContainsKey(item.Key))
                {
                    var child = transform.Find(item.Key.ToString());
                    if (child == null)
                    {
                        if (!warningGiven.Contains(item.Key))
                        {
                            Debug.LogError("No particle system for " + item.Key);
                            warningGiven.Add(item.Key);
                        }
                        continue;
                    }
                    var particle = child.GetComponent<ParticleSystem>();
                    if (particle == null)
                    {
                        if (!warningGiven.Contains(item.Key))
                        {
                            Debug.LogError("Malformed particle system for " + item.Key);
                            warningGiven.Add(item.Key);
                        }
                        continue;
                    }
                    flowParticles[item.Key] = particle;
                }
                foreach (var particle in item.Value)
                {
                    var emitParams = new ParticleSystem.EmitParams();
                    emitParams.position = GameMap.DFtoUnityTileCenter(particle.pos);
                    Color color = Color.white;
                    switch (item.Key)
                    {
                        case FlowType.Miasma:
                            break;
                        case FlowType.Steam:
                            break;
                        case FlowType.Mist:
                            break;
                        case FlowType.MaterialDust:
                            break;
                        case FlowType.MagmaMist:
                            break;
                        case FlowType.Smoke:
                            break;
                        case FlowType.Dragonfire:
                            color = ColorTemperature.Color(Mathf.Lerp(dragonColorTempMin, dragonColorTempMax, particle.density / 100.0f));
                            break;
                        case FlowType.Fire:
                            break;
                        case FlowType.Web:
                            break;
                        case FlowType.MaterialGas:
                            break;
                        case FlowType.MaterialVapor:
                            break;
                        case FlowType.OceanWave:
                            break;
                        case FlowType.SeaFoam:
                            break;
                        case FlowType.ItemCloud:
                        default:
                            color = ContentLoader.GetColor(particle.material);
                            break;
                    }
                    color.a = particle.density / 100f;
                    emitParams.startColor = color;
                    emitParams.applyShapeToPosition = true;
                    flowParticles[item.Key].Emit(emitParams, 1);
                }
            }
            yield return new WaitForSeconds(updateRate);
        }
    }

    internal void Clear()
    {
        flowMap.Clear();
        dirty = true;
    }
}
