using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementOption : MonoBehaviour
{
    public int choiceIndex;

    private void OnMouseDown()
    {
        HandleClick();
    }

    public void HandleClick()
    {
        DFConnection.Instance.SendCarefulMoveCommand(choiceIndex);
        Debug.Log("Sent choice index " + choiceIndex);
    }
}
