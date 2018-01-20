using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationManager : MonoBehaviour
{
    public static DecorationManager Instance { get; private set; }

    public GameObject Image;
    public GameObject Ring;
    public GameObject Spike;
    public GameObject Shape;

    private void Awake()
    {
        Instance = this;
    }
}
