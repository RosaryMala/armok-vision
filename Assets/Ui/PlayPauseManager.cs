using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayPauseManager : MonoBehaviour
{

    public GameObject playButton;

    bool paused = false;

    // Update is called once per frame
    void Update()
    {
        if (paused != DFConnection.Instance.DfPauseState)
        {
            paused = DFConnection.Instance.DfPauseState;
            if (playButton != null)
                playButton.GetComponentInChildren<Text>().text = paused ? "Resume" : "Pause";
        }
    }
}
