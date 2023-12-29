using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public GameObject particles;
    public bool bulletHole;
    public bool directional;

    public void SpawnParticles(GameObject collisionObject, RaycastHit hit)
    {
        if (particles == null) return;

        int count;

        if (collisionObject.TryGetComponent<Health>(out Health hp))
            count = (int)(hp.realDamage / 2.5f);
        else
            count = 1;

        for (int i = 0; i < count; i++)
        {
            GameObject newParticles = Instantiate(particles, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
            if (directional)
                newParticles.transform.LookAt(hit.point + hit.normal);
        }
    }

    public void SpawnParticles(GameObject collisionObject, Vector3 position, Vector3 direction)
    {
        if (particles == null) return;

        int count;

        if (collisionObject.TryGetComponent<Health>(out Health hp))
            count = (int)(hp.realDamage / 2.5f);
        else
            count = 1;

        for (int i = 0; i < count; i++)
        {
            GameObject newParticles = Instantiate(particles, position, Quaternion.identity) as GameObject;
            if (directional)
                newParticles.transform.LookAt(direction);
        }
    }
}
