using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class FadeAndDie : MonoBehaviour
{

    float startTime;
    public float startfade;
    public float fadelength;

    CanvasRenderer canRen;

    private void Awake()
    {
        canRen = GetComponent<CanvasRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        startTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        float relTime = Time.realtimeSinceStartup - startTime;
        if (relTime > (startfade + fadelength))
            Destroy(gameObject);
        else if(relTime > startfade)
        {
            canRen.SetAlpha(Mathf.Lerp(1, 0, (relTime - startfade) / fadelength));
        }
    }
}
