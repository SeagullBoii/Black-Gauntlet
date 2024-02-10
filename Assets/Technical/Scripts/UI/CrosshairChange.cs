using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CrosshairChange : MonoBehaviour
{
    public GameObject[] crosshairs;

    public float scopeChangeTime;

    public Weapons weaponScript;
    public WeaponAbilities weaponAbilities;

    bool changingToScope;
    bool changingToNormal;

    public int currentCrosshair;
    float aimingTimer;


    private void Update()
    {
        if (weaponAbilities.aiming)
        {
            if (!changingToScope)
            {
                aimingTimer += Time.deltaTime;
                if (aimingTimer >= scopeChangeTime)
                    ChangeCrosshair(0);
            }
        }
        else
        {
            ChangeCrosshair(weaponScript.currentWeaponID + 1);

            aimingTimer = 0;
            changingToScope = false;
        }

        weaponScript.crosshair = crosshairs[currentCrosshair];
    }

    private void ChangeCrosshair(int id)
    {

        for (int i = 0; i < crosshairs.Length; i++)
            crosshairs[i].SetActive(false);

        crosshairs[id].SetActive(true);
        currentCrosshair = id;
    }

}
