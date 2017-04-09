using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsQuestion : MonoBehaviour
{
    public GoogleAnalyticsV4 analytics;

    // Use this for initialization
    void Start()
    {
        if (GameSettings.Instance.game.analytics != GameSettings.AnalyticsChoice.Unknown)
            gameObject.SetActive(false);
    }
    
    public void PushYes()
    {
        GameSettings.Instance.game.analytics = GameSettings.AnalyticsChoice.Yes;
        analytics.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    public void PushNo()
    {
        GameSettings.Instance.game.analytics = GameSettings.AnalyticsChoice.No;
        gameObject.SetActive(false);
    }
}
