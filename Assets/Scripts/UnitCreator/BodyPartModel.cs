using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartModel : MonoBehaviour
{
    [System.Serializable]
    public class ChildLocation
    {
        public string category;
        public string token;
        public Vector3 pos1;
        public Vector3 pos2;
        public Vector3 posDefault;
        public Vector3 rot1;
        public Vector3 rot2;
        public Vector3 rotDefault;
    }


    public float canonVolume = 1;
    public List<ChildLocation> childParts = new List<ChildLocation>();
}
