using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RevolverGrenade : MonoBehaviour
{
    [Header("Stats")]
    public float time = 2f;
    public float damage = 20;
    public float radius = 6;
    public float explosionForce = 700f;

    [Header("References")]
    public GameObject explosionFX;
    public GameObject redExplosionFX;
    public GameObject damagePopupPrefab;
    public LayerMask ignore;
    public Scythe scythe;

    public enum ExplosionType
    {
        Red, Normal
    }

    [HideInInspector]
    public bool hasCollided;

    public void Explode(ExplosionType type)
    {

        float forceMultiplier = 1.25f;
        float radiusAddition = 0f;
        float damageMultiplier = 1;
        GameObject fx = explosionFX;
        float maxShakeDistMultiplier = 1;
        float shakeVibratoMultiplier = 1;
        switch (type) {
            case ExplosionType.Red: 
                forceMultiplier = 1.25f; 
                radiusAddition = 6f;
                damageMultiplier = 1.5f;
                fx = redExplosionFX;
                maxShakeDistMultiplier = 1.5f;
                shakeVibratoMultiplier = 1.5f;
                break;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        ArrayList hitObjects = new ArrayList();
        foreach (Collider nearbyObject in colliders)
        {
            CameraShake.Invoke(0.5f, 1, 5, 200 * shakeVibratoMultiplier, transform, 50 * maxShakeDistMultiplier);

            if (nearbyObject.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(explosionForce * forceMultiplier, transform.position, radius + radiusAddition);
            }

            if (nearbyObject.gameObject.TryGetComponent<Health>(out Health hp) && !hitObjects.Contains(nearbyObject.gameObject))
            {
                hp.DealDamage(damage * damageMultiplier, true);
                hp.scythe = scythe;
                SpawnDamagePopup(hp, nearbyObject.ClosestPoint(transform.position), nearbyObject.gameObject);
                hitObjects.Add(nearbyObject.gameObject);
            }
            if (nearbyObject.GetComponent<HitEffect>())
                nearbyObject.GetComponent<HitEffect>().SpawnParticles(nearbyObject.gameObject, nearbyObject.ClosestPoint(transform.position), transform.position);

            hitObjects.Clear();
        }

        GameObject explosion = Instantiate(fx, transform.position, transform.rotation);
        Destroy(explosion, 3f);
        Destroy(transform.gameObject);
    }

    private void BaseExplosion() {
        Explode(ExplosionType.Normal);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != ignore)
        {
            hasCollided = true;
            Invoke(nameof(BaseExplosion), time);
        }

    }

    private void SpawnDamagePopup(Health health, Vector3 position, GameObject collisionObject)
    {
        if (!health.damagePopup) return;

        GameObject damagePopupObject = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        damagePopupObject.GetComponent<DamagePopup>().damage = health.realDamage;

        float xDistance = Mathf.Abs(position.x - Camera.main.transform.position.x);
        float yDistance = Mathf.Abs(position.y - Camera.main.transform.position.y);
        float zDistance = Mathf.Abs(position.z - Camera.main.transform.position.z);
        Vector3 distance = new Vector3(xDistance, yDistance, zDistance);

        damagePopupObject.transform.localScale = new Vector3(distance.magnitude, distance.magnitude, distance.magnitude);

        float fontSize = damagePopupObject.transform.localScale.magnitude;
        if (fontSize < 15) fontSize = 15;
        if (damagePopupObject.TryGetComponent<TMP_Text>(out TMP_Text tmpText)) tmpText.fontSize = 2.5f * fontSize;
    }


}
