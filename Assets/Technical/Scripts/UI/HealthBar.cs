using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health health;
    Slider slider;
    float currentVelocity = 0;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = health.maxHealth;
    }

    private void FixedUpdate()
    {
        if (slider.value != health.health)
        {

            float currentHealth = Mathf.SmoothDamp(slider.value, health.health, ref currentVelocity, 10 * Time.deltaTime);
            slider.value = currentHealth;
        }
    }
}
