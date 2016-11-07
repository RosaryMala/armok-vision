using UnityEngine;
using System.Collections;

public class PlayPauseManager : MonoBehaviour
{

    public GameObject playButton;
    public GameObject pauseButton;

    bool paused = false;

    void Start()
    {
        playButton.SetActive(paused);
        pauseButton.SetActive(!paused);
    }

    // Update is called once per frame
    void Update()
    {
        if (paused != DFConnection.Instance.DfPauseState)
        {
            paused = DFConnection.Instance.DfPauseState;
            playButton.SetActive(paused);
            pauseButton.SetActive(!paused);
        }
    }
}
