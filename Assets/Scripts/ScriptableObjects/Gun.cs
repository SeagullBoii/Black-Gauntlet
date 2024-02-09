using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    [Header("Stats")]
    public string weaponName;
    public float damage;
    public float criticalMultiplier;
    public float fireRate;
    public float bloom;
    public float weaponModCooldown;
    public float knockbackForce;
    public FiringState firingState;

    [Header("Recoil")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;

    public float gunRecoilX;
    public float gunRecoilY;
    public float gunRecoilZ;

    public float positionalRecoilZ;
    public float shotRecoilRatio;

    public float returnSpeed;

    [Header("Effects")]
    public float pitchRandomization;
    public AudioClip gunShotSound;
    public AudioClip abilitySound;

    public GameObject prefab;
    public GameObject muzzleFlash;
    public GameObject bulletTrail;

    public enum FiringState
    {
        semi,
        burst,
        auto
    }

}
