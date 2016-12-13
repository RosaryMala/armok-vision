using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [Flags]
    public enum MaterialFlags
    {
        None = 0,
        Contaminants = 1
    }
    public enum TransparencyType
    {
        Opaque,
        Stencil,
        Transparent
    }

    [SerializeField]
    Material baseStandardMaterial;

    [SerializeField]
    Material baseSplatMaterial;
}
