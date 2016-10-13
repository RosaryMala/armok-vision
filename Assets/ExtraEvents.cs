using System;
using UnityEngine;
using UnityEngine.Events;

public class ExtraEvents : MonoBehaviour
{
    public OnStartEvent OnStart;

    // Use this for initialization
    void Start()
    {
        OnStart.Invoke(gameObject);
    }

    [Serializable]
    public class OnStartEvent : UnityEvent<GameObject>
    {

    }
}
