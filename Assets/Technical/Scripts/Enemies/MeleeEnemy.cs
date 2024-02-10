using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Patrolling")]
    UnityEngine.Vector3 walkPoint;
    bool walkPointSet;
    [SerializeField] float walkPointRange;
    [SerializeField] float timeBetweenPatrols;
    [SerializeField] float patrolSpeed;

    [Header("Attacking")]
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] float chaseSpeed;
    bool alreadyAttacked;
    bool seenPlayer;

    [Header("States")]
    [SerializeField] float sightRange, attackRange;
    bool playerInSightRange, playerInAttackRange;

    [Header("References")]

    [SerializeField] LayerMask ground, playerMask;
    [SerializeField] Animator animator;
    Transform player;
    NavMeshAgent agent;
    Rigidbody rb;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!seenPlayer) playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        else playerInSightRange = true;
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (playerInSightRange) seenPlayer = true;

        //States
        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) Attack();

        if (!playerInAttackRange && alreadyAttacked) alreadyAttacked = false;

        animator.SetFloat("Speed", agent.velocity.magnitude / chaseSpeed);
    }

    private void FixedUpdate()
    {
        UnityEngine.Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1)
        {
            walkPointSet = false;
            Invoke(nameof(ResetPatrols), timeBetweenPatrols);
        }
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchForWalkPoint();
    }

    private void ResetPatrols()
    {
        walkPointSet = false;
    }

    private void SearchForWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        //Finding The WalkPoint
        walkPoint = new UnityEngine.Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2, ground)) walkPointSet = true;

        agent.speed = patrolSpeed;
        if (walkPointSet) agent.SetDestination(walkPoint);
    }

    private void ChasePlayer()
    {
        walkPointSet = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        transform.eulerAngles = new UnityEngine.Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

        if (!alreadyAttacked)
        {
            animator.SetTrigger("Attack");
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}