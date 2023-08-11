using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient Gradient;
    public Image fill;
    // [SerializeField] private GameObject player;

    void Start()
    {
        
    }

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
        slider.value = value;
        fill.color = Gradient.Evaluate(1f);
    }

    public void SetValue(float value)
    {
        slider.value = value;
        fill.color = Gradient.Evaluate(slider.normalizedValue);
    }
}
