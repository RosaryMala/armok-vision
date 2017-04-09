using UnityEngine;
using System.Collections;

public class Survey : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.GetInt("Survey2", 0) == 1)
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenSurvey()
    {
        Application.OpenURL("https://goo.gl/forms/Olo2LONurpjiSLTz2");
    }

    public void ClosePanel()
    {
        PlayerPrefs.SetInt("Survey2", 1);
        gameObject.SetActive(false);
    }
}
