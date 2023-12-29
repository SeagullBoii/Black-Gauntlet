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

    [HideInInspector]
    public bool hasCollided;

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        ArrayList hitObjects = new ArrayList();
        foreach (Collider nearbyObject in colliders)
        {

            if (nearbyObject.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }

            if (nearbyObject.gameObject.TryGetComponent<Health>(out Health hp) && !hitObjects.Contains(nearbyObject.gameObject))
            {
                hp.DealDamage(damage, true);
                hp.scythe = scythe;
                SpawnDamagePopup(hp, nearbyObject.ClosestPoint(transform.position), nearbyObject.gameObject);
                hitObjects.Add(nearbyObject.gameObject);
            }
            if (nearbyObject.GetComponent<HitEffect>())
                nearbyObject.GetComponent<HitEffect>().SpawnParticles(nearbyObject.gameObject, nearbyObject.ClosestPoint(transform.position), transform.position);

            hitObjects.Clear();
        }

        GameObject explosion = Instantiate(explosionFX, transform.position, transform.rotation);
        Destroy(explosion, 3f);
        Destroy(transform.gameObject);
    }

    public void ExplodeRed()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in colliders)
        {

            if (nearbyObject.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(explosionForce * 1.25f, transform.position, radius + 6);
            }
            if (nearbyObject.gameObject.TryGetComponent<Health>(out Health hp))
            {
                hp.DealDamage(damage * 1.5f, true);
                hp.scythe = scythe;
                SpawnDamagePopup(hp, nearbyObject.ClosestPoint(transform.position), nearbyObject.gameObject);
            }

            if (nearbyObject.GetComponent<HitEffect>())
                nearbyObject.GetComponent<HitEffect>().SpawnParticles(nearbyObject.gameObject, nearbyObject.ClosestPoint(transform.position), transform.position);

        }

        GameObject explosion = Instantiate(redExplosionFX, transform.position, transform.rotation);
        Destroy(explosion, 3f);
        Destroy(transform.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != ignore)
        {
            hasCollided = true;
            Invoke(nameof(Explode), time);
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
