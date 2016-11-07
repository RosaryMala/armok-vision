using UnityEngine;
using System.Collections;

public class PanelSlider : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public bool isOut = true;

    public void Slide()
    {
        if (isOut)
        {
            anim.Play("PanelSlideIn");
            isOut = false;
        }
        else
        {
            anim.Play("PanelSlideOut");
            isOut = true;
        }
    }
}
