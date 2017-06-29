using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsQuestion : MonoBehaviour
{
    public GoogleAnalyticsV4 analytics;

    // Use this for initialization
    void Start()
    {
        if (GameSettings.Instance.game.analytics == GameSettings.AnalyticsChoice.Unknown)
            ModalPanel.Instance.Choice(
                "Allow Armok Vision to collect anonymous usage data to help development?\n"+
"\n" +
"Data collected includes:\n" +
" *Armok Vision Version\n" +
" * Dwarf Fortress Verrsion\n" +
" * DFHack Plugin Version\n" +
" * CPU ID\n" +
" * Graphics Card Model\n" +
" * Operating System.\n" +
"\n" +
"If you chose no, no data will be sent.\n", PushYes, PushNo);
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
