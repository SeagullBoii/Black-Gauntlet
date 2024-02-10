using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScytheChargeBar : MonoBehaviour
{
    public Scythe scythe;
    Slider slider;
    float currentVelocity = 0;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = scythe.maxCharge;
    }

    private void FixedUpdate()
    {
        if (slider.value != scythe.charge)
        {
            float difference = Mathf.Abs(slider.value - scythe.charge);
            float currentCharge = Mathf.SmoothDamp(slider.value, scythe.charge, ref currentVelocity, 1 * 10 * Time.deltaTime);
            slider.value = currentCharge;
        }
    }
}
