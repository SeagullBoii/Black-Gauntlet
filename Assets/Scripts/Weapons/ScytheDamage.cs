using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScytheDamage : MonoBehaviour
{
    [Header("Damage")]
    public float damage;
    public float chargedDamage;

    [Header("Collider")]
    public Vector3 normalColliderSize;
    public Vector3 normalColliderPos;
    public Vector3 chargedColliderSize;
    public Vector3 chargedColliderPos;

    [Header("Healing")]
    public float count;
    public float healthMultiplier;
    public GameObject bloodOrbPrefab;

    [Header("References")]
    public GameObject damagePopupPrefab;
    public Scythe scytheScript;

    [HideInInspector]
    public bool charged;
    [HideInInspector]
    public bool resettingCharge;

    BoxCollider hitBox;
    ArrayList hitObjects = new ArrayList();

    private void Start()
    {
        hitBox = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (!hitBox.enabled)
            hitObjects.Clear();

        if (resettingCharge && !scytheScript.isSwinging) resettingCharge = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<HurtBox>(out HurtBox hurtBox) && !hitObjects.Contains(hurtBox.health.gameObject))
        {
            //Damage
            float damageToDeal = charged || (resettingCharge && scytheScript.isSwinging) ? chargedDamage : damage;
            hurtBox.scythe = scytheScript;
            if (!charged)
                hurtBox.DealNormalDamage(damageToDeal, !charged, other.ClosestPoint(transform.position));
            else
                hurtBox.DealDamage(damageToDeal, charged, DamageType.DamageTypes.critical, 1, other.ClosestPoint(transform.position));

            //VFX
            if (other.GetComponent<HitEffect>())
                other.GetComponent<HitEffect>().SpawnParticles(other.gameObject, other.ClosestPoint(transform.position), transform.position);

            //Charge
            if (scytheScript.charge >= scytheScript.maxCharge)
            {
                scytheScript.charge = 0;
                resettingCharge = true;
            }

            //Spawn HP Orbs
            if (charged) SpawnHealthOrbs(other);


            hitObjects.Add(hurtBox.health.gameObject);
        }
        else if (other.gameObject.TryGetComponent<Health>(out Health health) && !hitObjects.Contains(other.gameObject))
        {
            //Damage
            float damageToDeal = charged || (resettingCharge && scytheScript.isSwinging) ? chargedDamage : damage;
            health.DealDamage(damageToDeal, !charged, charged ? DamageType.DamageTypes.critical : DamageType.DamageTypes.normal);
            health.scythe = scytheScript;

            //VFX
            health.SpawnDamagePopup(other.ClosestPoint(transform.position));
            if (other.GetComponent<HitEffect>())
                other.GetComponent<HitEffect>().SpawnParticles(other.gameObject, other.ClosestPoint(transform.position), transform.position);


            //Charge
            if (scytheScript.charge >= scytheScript.maxCharge)
            {
                scytheScript.charge = 0;
                resettingCharge = true;
            }

            //Spawn HP Orbs
            if (charged) SpawnHealthOrbs(other);


            hitObjects.Add(other.gameObject);
        }
    }

    public void SpawnHealthOrbs(Collider other)
    {
        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(-0.6f, 0.6f);
            float randomY = Random.Range(-0.6f, 0.6f);
            float randomZ = Random.Range(-0.6f, 0.6f);

            Vector3 randomPos = new Vector3(randomX, randomY, randomZ);

            GameObject healthOrb = Instantiate(bloodOrbPrefab, other.ClosestPoint(transform.position) + randomPos, Quaternion.identity);
            healthOrb.GetComponent<Rigidbody>().AddExplosionForce(10, other.ClosestPoint(transform.position), 1);
            if (healthOrb.GetComponent<HealthOrb>()) healthOrb.GetComponent<HealthOrb>().health = chargedDamage * healthMultiplier;
        }
    }

}
