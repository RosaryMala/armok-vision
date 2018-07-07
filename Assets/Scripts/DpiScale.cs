using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class DpiScale : MonoBehaviour
{
    [SerializeField]
    float _dpi = 0;

    // Update is called once per frame
    void Update()
    {
        if(Screen.dpi != _dpi)
        {
            _dpi = Screen.dpi;
            GetComponent<CanvasScaler>().scaleFactor = _dpi / 96.0f;
        }

    }
}
