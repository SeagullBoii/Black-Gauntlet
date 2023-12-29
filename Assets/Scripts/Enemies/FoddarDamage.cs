using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] Collider baseCollider;
    [SerializeField] GameObject[] decals;
    ArrayList hitObjects = new ArrayList();


    private void Update()
    {
        if (!baseCollider.enabled)
        {
            hitObjects.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Health>(out Health hp) && other.tag == "Player")
        {
            hp.DealDamage(damage);
            hitObjects.Add(other.gameObject);
            foreach (GameObject decal in decals)
            {
                decal.SetActive(true);
            }
        }
    }
}
