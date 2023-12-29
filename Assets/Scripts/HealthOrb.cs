using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HealthOrb : MonoBehaviour
{
    public float health;
    public float speed;
    public float range;

    public LayerMask ground;

    public GameObject particles;
    public TrailRenderer trail;

    Rigidbody rb;
    bool chasingPlayer;
    bool hasHealed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bool grounded = Physics.Raycast(transform.position, Vector3.down, transform.localScale.y * 0.5f + 0.2f, ground);
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider objects in colliders)
        {

            if (objects.tag == "Player")
            {
                //Player chasing
                Vector3 targetPos = objects.transform.position;
                chasingPlayer = objects.TryGetComponent<Health>(out Health hp) && hp.health != hp.maxHealth && Mathf.Abs((targetPos - transform.position).magnitude) <= range;

                //Movement
                if (chasingPlayer && !hasHealed)
                    MoveToPlayer(objects.transform);

                //Reset
                rb.isKinematic = grounded && !chasingPlayer;
            }
        }
    }

    private void MoveToPlayer(Transform target)
    {
        rb.velocity = transform.forward * speed;
        transform.LookAt(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (chasingPlayer)
        {
            if (other.TryGetComponent<Health>(out Health hp) && other.tag == "Player")
            {
                if (!hasHealed)
                {
                    hasHealed = true;
                    hp.health += health;
                    Mathf.Clamp(hp.health, 0, hp.maxHealth);
                }


                trail.enabled = false;
                particles.SetActive(true);
                GetComponent<MeshRenderer>().enabled = false;
                Destroy(gameObject, 1);
            }
        }
    }
}
