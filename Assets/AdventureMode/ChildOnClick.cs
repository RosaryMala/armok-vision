using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildOnClick : MonoBehaviour
{
    public MovementOption parent;
    private void OnMouseDown()
    {
        parent.HandleClick();
    }
}
