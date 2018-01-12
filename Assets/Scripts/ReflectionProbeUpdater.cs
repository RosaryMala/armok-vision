using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReflectionProbe))]
public class ReflectionProbeUpdater : MonoBehaviour
{
    ReflectionProbe probe;
    public float renderDistance = 20;

    private void Awake()
    {
        probe = GetComponent<ReflectionProbe>();
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(ProbeUpdater());
    }

    IEnumerator ProbeUpdater()
    {
        if((Camera.main.transform.position - transform.position).magnitude < renderDistance)
            probe.RenderProbe();
        yield return new WaitForSecondsRealtime(1);
    }

}
