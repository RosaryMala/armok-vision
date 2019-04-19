using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ProgressBar : MonoBehaviour
{
    public RectTransform slider;
    public Text progressText;
    [Range(0,1)]
    [SerializeField]
    private float _value;

    private void OnValidate()
    {
        var max = slider.anchorMax;
        slider.anchorMax = new Vector2(_value, max.y);
    }

    public void SetProgress(float amount, string text)
    {
        _value = amount;
        OnValidate();
        progressText.text = text;
    }
    public void SetProgress(string text)
    {
        progressText.text = text;
    }
    public void SetProgress(float amount)
    {
        _value = amount;
        OnValidate();
    }
}
