using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunObjects : MonoBehaviour
{
    [Header("Gun Glow")]
    public GameObject[] glowyParts;
    public Color emissionColor;
    public float emissionCooldownDifference;
    public float minEmissionValue;
    public float maxEmissionValue;

    [Header("Slider Glow")]
    public float sliderMaxEmission;
    public float minTransparency;
    public float maxTransparency;
    public Color sliderEmissionColor;


    [Header("References")]
    public Transform muzzleFlash;
    public Slider abilitySlider;
    public WeaponAbilities weaponAbilities;
    public Weapons weapons;


    private void Update()
    {
        SliderValue();
        SliderGlow();
        GunGlow();
    }

    private void SliderValue()
    {
        if (abilitySlider != null)
        {
            abilitySlider.maxValue = weapons.loadout[weapons.currentWeaponID].weaponModCooldown;
            if (weapons.currentWeaponID == 0)
                abilitySlider.value = abilitySlider.maxValue - weaponAbilities.firstWeaponModCooldown;
            else if (weapons.currentWeaponID == 2)
                abilitySlider.value = abilitySlider.maxValue - weaponAbilities.thirdWeaponModCooldown;
        }
    }

    Color sliderEmission = new Color();
    Color transparencyColor = new Color();
    private void SliderGlow()
    {
        if (abilitySlider == null) return;
        //Emission
        float emissionScalingFactor = (sliderMaxEmission - minEmissionValue) / weapons.loadout[weapons.currentWeaponID].weaponModCooldown;
        float emissionYIntercept = minEmissionValue;

        sliderEmission = sliderEmissionColor * Mathf.Pow(2, emissionScalingFactor * abilitySlider.value + emissionYIntercept);
        abilitySlider?.fillRect.GetComponent<Image>().material.SetColor("_GlowColor", sliderEmission);


        //Lightness
        float lightnessScalingFactor = (maxTransparency - minTransparency) / weapons.loadout[weapons.currentWeaponID].weaponModCooldown;
        float lightnessYIntercept = minTransparency;

        transparencyColor = abilitySlider.fillRect.GetComponent<Image>().material.GetColor("_Color");
        transparencyColor.a = lightnessScalingFactor * abilitySlider.value + lightnessYIntercept;

        abilitySlider?.fillRect.GetComponent<Image>().material.SetColor("_Color", transparencyColor);
    }

    float emissionScalingFactor;
    float emissionYIntercept;
    private void GunGlow()
    {
        if (glowyParts == null) return;
        foreach (GameObject glowyPart in glowyParts)
        {
            emissionScalingFactor = (maxEmissionValue - minEmissionValue) / weapons.loadout[weapons.currentWeaponID].weaponModCooldown;
            emissionYIntercept = minEmissionValue;

            Material material = glowyPart.GetComponent<MeshRenderer>().material;
            Color emission = emissionColor * Mathf.Pow(2, emissionScalingFactor * abilitySlider.value + emissionYIntercept);
            material.SetColor("_EmissionColor", emission);
        }
    }
}
