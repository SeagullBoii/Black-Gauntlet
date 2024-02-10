using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HurtBox : MonoBehaviour
{
    [SerializeField] bool critical;
    public Health health;
    public Collider hitCollider;

    [HideInInspector] public Scythe scythe;

    public void DealDamage(float damage, bool charge, DamageType.DamageTypes type, float multiplier, RaycastHit hit)
    {
        if (type == DamageType.DamageTypes.normal && critical && multiplier != 1) type = DamageType.DamageTypes.critical;

        float criticalMultiplier = type == DamageType.DamageTypes.critical ? multiplier : 1;

        health.scythe = scythe;
        health.DealDamage(damage * criticalMultiplier, charge, type);
        health.SpawnDamagePopup(hit, type);
    }

    public void DealDamage(float damage, bool charge, DamageType.DamageTypes type, float multiplier, Vector3 position)
    {
        if (type == DamageType.DamageTypes.normal && critical && multiplier != 1) type = DamageType.DamageTypes.critical;

        float criticalMultiplier = type == DamageType.DamageTypes.critical ? multiplier : 1;

        health.scythe = scythe;
        health.DealDamage(damage * criticalMultiplier, charge, type);
        health.SpawnDamagePopup(position, type);

    }

    public void DealDamage(float damage, bool charge, DamageType.DamageTypes type, float multiplier)
    {
        if (type == DamageType.DamageTypes.normal && critical && multiplier != 1) type = DamageType.DamageTypes.critical;

        float criticalMultiplier = type == DamageType.DamageTypes.critical ? multiplier : 1;

        health.scythe = scythe;
        health.DealDamage(damage * criticalMultiplier, charge, type);
    }

    public void DealNormalDamage(float damage, bool charge, Vector3 position)
    {
        health.scythe = scythe;
        health.DealDamage(damage, charge);
        health.SpawnDamagePopup(position);
    }
}
