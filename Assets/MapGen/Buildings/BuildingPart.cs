using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class BuildingPart : MonoBehaviour
    {
        public string item;
    }
}