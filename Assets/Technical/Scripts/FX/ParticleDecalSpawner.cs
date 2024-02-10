using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticleDecalSpawner : MonoBehaviour
{
    public GameObject decal;

    ParticleSystem system;
    List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        system = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    [ExecuteInEditMode]
    private void OnParticleCollision(GameObject other)
    {
        int collisionEventNumber = system.GetCollisionEvents(other, collisionEvents);
        Instantiate(decal);
        for (int i = 0; i < collisionEventNumber; i++)
        {
            GameObject newDecal = Instantiate(decal, collisionEvents[i].intersection, Quaternion.LookRotation(collisionEvents[i].normal * -1));
            newDecal.TryGetComponent<ChildGetter>(out ChildGetter children);
            //newDecal.transform.position = collisionEvents[i].intersection;
            foreach (GameObject child in children.children)
            {
                child.transform.localEulerAngles = new Vector3(child.transform.localEulerAngles.x, child.transform.localEulerAngles.y, Random.Range(0, 360));
            }

        }


    }
}
