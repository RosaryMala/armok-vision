using UnityEngine;
using System.Collections;

public class BringToFront : MonoBehaviour
{

    void OnEnable()
    {
        transform.SetAsLastSibling();
    }
}