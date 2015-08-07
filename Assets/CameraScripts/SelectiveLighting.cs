using UnityEngine;
using System.Collections;

public class SelectiveLighting : MonoBehaviour {

    public Light[] OnLights;
    public Light[] OffLights;
    private bool[] OnLightsStatus;
    private bool[] OffLightsStatus;

    void OnPreCull()
    {
        InitStatus();
        for (int i = 0; i < OnLights.Length; i++)
        {
            OnLightsStatus[i] = OnLights[i].enabled;
            OnLights[i].enabled = true;
        }
        for (int i = 0; i < OffLights.Length; i++)
        {
            OffLightsStatus[i] = OffLights[i].enabled;
            OffLights[i].enabled = false;
        }
    }

    void OnPostRender()
    {
        for (int i = 0; i < OnLights.Length; i++)
        {
            OnLights[i].enabled = OnLightsStatus[i];
        }
        for (int i = 0; i < OffLights.Length; i++)
        {
            OffLights[i].enabled = OffLightsStatus[i];
        }
    }

    void InitStatus()
    {
        if (OnLightsStatus == null || OnLightsStatus.Length != OnLights.Length)
            OnLightsStatus = new bool[OnLights.Length];
        if (OffLightsStatus == null || OnLightsStatus.Length != OffLights.Length)
            OffLightsStatus = new bool[OffLights.Length];
    }

}
