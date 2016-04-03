using UnityEngine;
using System.Collections;

public class PosTest : MonoBehaviour
{
    public UnityEngine.UI.Text posText;

    // Use this for initialization
    void Start()
    {
        Vector3 pos = transform.localPosition;
        posText.text = "Pos: " + pos.z.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.localPosition;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            pos.z += 0.1f;
            posText.text = "Pos: " + pos.z.ToString();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            pos.z -= 0.1f;
            posText.text = "Pos: " + pos.z.ToString();
        }
        transform.localPosition = pos;
    }
}
