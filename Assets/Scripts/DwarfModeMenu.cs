using proto.enums.ui_sidebar_mode;
using UnityEngine;

public class DwarfModeMenu : MonoBehaviour
{
    [SerializeField]
    private ui_sidebar_mode mode;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var sidebar = DFConnection.Instance.SidebarState;
        if(sidebar != null && mode != sidebar.mode)
        {
            mode = sidebar.mode;
            Debug.Log("Menu changed to " + mode);
        }
    }
}
