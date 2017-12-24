using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiSwitcher : MonoBehaviour
{

    public List<GameObject> dwarfUI = new List<GameObject>();
    public List<GameObject> adventureUI = new List<GameObject>();

    dfproto.GetWorldInfoOut.Mode prevMode = dfproto.GetWorldInfoOut.Mode.MODE_LEGENDS;

    // Use this for initialization
    void Start()
    {
        foreach (var item in dwarfUI)
        {
            item.SetActive(false);
        }
        foreach (var item in adventureUI)
        {
            item.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(DFConnection.Instance.WorldMode != prevMode)
        {
            prevMode = DFConnection.Instance.WorldMode;
            switch (prevMode)
            {
                case dfproto.GetWorldInfoOut.Mode.MODE_DWARF:
                    foreach (var item in dwarfUI)
                    {
                        item.SetActive(true);
                    }
                    foreach (var item in adventureUI)
                    {
                        item.SetActive(false);
                    }
                    break;
                case dfproto.GetWorldInfoOut.Mode.MODE_ADVENTURE:
                    foreach (var item in dwarfUI)
                    {
                        item.SetActive(false);
                    }
                    foreach (var item in adventureUI)
                    {
                        item.SetActive(true);
                    }
                    break;
                default:
                    foreach (var item in dwarfUI)
                    {
                        item.SetActive(false);
                    }
                    foreach (var item in adventureUI)
                    {
                        item.SetActive(false);
                    }
                    break;
            }
        }
    }
}
