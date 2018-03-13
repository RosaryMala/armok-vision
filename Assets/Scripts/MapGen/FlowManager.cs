using DFHack;
using RemoteFortressReader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    Dictionary<DFCoord, List<FlowInfo>> flowMap = new Dictionary<DFCoord, List<FlowInfo>>();
    Dictionary<DFCoord, List<FlowInfo>> tileFlowMap = new Dictionary<DFCoord, List<FlowInfo>>();
    private bool dirty;

    Dictionary<FlowType, List<FlowInfo>> flowTypes = new Dictionary<FlowType, List<FlowInfo>>();

    Dictionary<FlowType, ParticleSystem> flowParticles = new Dictionary<FlowType, ParticleSystem>();

    public float dragonColorTempMin = 0;
    public float dragonColorTempMax = 22495;

    public float updateRate = 0.1f;

    public static FlowManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        Instance = this;
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

    /// <summary>
    /// Since tiles aren't updated as often as regular flows, they need to be in a separate list.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="flows"></param>
    public void SetTileFlows(DFCoord pos, List<FlowInfo> flows)
    {
        if (flows == null || flows.Count == 0)
            tileFlowMap.Remove(pos);
        else
            tileFlowMap[pos] = flows;
        dirty = true;
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
                    if (flow.density > 0)
                        flowTypes[flow.type].Add(flow);
                }
            }
            foreach (var pos in tileFlowMap)
            {
                foreach (var flow in pos.Value)
                {
                    if (!flowTypes.ContainsKey(flow.type))
                        flowTypes[flow.type] = new List<FlowInfo>();
                    if (flow.density > 0)
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
                    if (!GameMap.IsInVisibleArea(particle.pos))
                        continue;
                    var emitParams = new ParticleSystem.EmitParams();
                    emitParams.position = GameMap.DFtoUnityTileCenter(particle.pos);
                    Color color = Color.white;
                    switch (item.Key)
                    {
                        case FlowType.Dragonfire:
                            color = ColorTemperature.Color(Mathf.Lerp(dragonColorTempMin, dragonColorTempMax, particle.density / 100.0f));
                            break;
                        case FlowType.Miasma:
                        case FlowType.Steam:
                        case FlowType.Mist:
                        case FlowType.Smoke:
                        case FlowType.MagmaMist:
                        case FlowType.CampFire:
                        case FlowType.Fire:
                        case FlowType.OceanWave:
                        case FlowType.SeaFoam:
                            color = Color.white;
                            break;
                        case FlowType.ItemCloud:
                            color = ContentLoader.GetColor(particle.item, particle.material);
                            flowParticles[item.Key].customData.SetVector(ParticleSystemCustomData.Custom1, 0, new ParticleSystem.MinMaxCurve(ImageManager.Instance.GetItemTile(particle.item)));
                            flowParticles[item.Key].customData.SetVector(ParticleSystemCustomData.Custom1, 1, new ParticleSystem.MinMaxCurve(ContentLoader.GetPatternIndex(particle.material)));
                            break;
                        case FlowType.MaterialGas:
                        case FlowType.MaterialVapor:
                        case FlowType.MaterialDust:
                        case FlowType.Web:
                        default:
                            color = ContentLoader.GetColor(particle.material);
                            break;
                    }
                    if (item.Key == FlowType.ItemCloud)
                    {
                        if (UnityEngine.Random.Range(0, 100) > particle.density)
                            continue;
                    }
                    else
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

    private void OnDrawGizmosSelected()
    {
        foreach (var pos in flowMap)
        {
            foreach (var particle in pos.Value)
            {
                if (particle.density <= 0)
                    continue;
                switch (particle.type)
                {
                    case FlowType.Dragonfire:
                    case FlowType.Web:
                        Gizmos.DrawLine(GameMap.DFtoUnityTileCenter(particle.pos), GameMap.DFtoUnityTileCenter(particle.dest));
                        break;
                    default:
                        Gizmos.DrawRay(GameMap.DFtoUnityTileCenter(particle.pos), Vector3.up);
                        break;
                }
            }
        }
    }
}
