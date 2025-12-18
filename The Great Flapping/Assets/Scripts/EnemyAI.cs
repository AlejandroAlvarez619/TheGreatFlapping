using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    public Animator animator;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange;
    public float attackRange;

    public bool playerInSightRange;
    public bool playerInAttackRange;

    public float patrolRunSpeed = 0.5f;
    public float chaseRunSpeed = 1.5f;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange * 0.5f;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        playerInSightRange = distanceToPlayer <= sightRange;
        playerInAttackRange = distanceToPlayer <= attackRange;

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        else if (playerInSightRange && !playerInAttackRange) Chase();
        else if (playerInAttackRange && playerInSightRange) Attack();
    }


    private void Patrol()
    {
        agent.speed = patrolRunSpeed;

        if (!walkPointSet) SearchForWalkPoint();

        if (walkPointSet)
        {
            Debug.Log("Moving to walkPoint: " + walkPoint);
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;

        animator.SetFloat("RunSpeed", patrolRunSpeed);
    }

    private void Chase()
    {
        agent.speed = chaseRunSpeed;
        agent.SetDestination(player.position);

        animator.SetFloat("RunSpeed", chaseRunSpeed);
    }

    private void Attack()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            // Add Attack Code
            // animator.SetTrigger("Attack");
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                Destroy(player.gameObject);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void SearchForWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

}
