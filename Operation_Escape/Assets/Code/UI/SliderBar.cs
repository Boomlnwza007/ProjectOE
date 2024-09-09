using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider slider;
    private float value;
    public GameObject bar;
    private void Update()
    {
        if (slider.value != value)
        {
            slider.value = Mathf.Lerp(slider.value, value, 0.05f);
        }
    }
    public void SetMax(float _value)
    {
        slider.maxValue = _value;
        slider.value = _value;
        _value = value;
    }

    public void SetValue(float _value)
    {
        value = _value;
    }

    public void Off()
    {
        bar.SetActive(false);
    }
}
