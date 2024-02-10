using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponModCDBar : MonoBehaviour
{
    public Weapons weapons;
    public WeaponAbilities weaponAbilities;
    Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.maxValue = weapons.loadout[weapons.currentWeaponID].weaponModCooldown;
        if (weapons.currentWeaponID == 0)
            slider.value = slider.maxValue - weaponAbilities.firstWeaponModCooldown;
        else if (weapons.currentWeaponID == 2)
            slider.value = slider.maxValue - weaponAbilities.thirdWeaponModCooldown;
    }
}
