using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EnemyAttack
{
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;
    public Transform[] patrolPoints;
    public Transform player;
    private int currentPoint = 0;
    private bool chasing = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            chasing = true;
        }
        else if (distanceToPlayer > detectionRange + 1f)
        {
            chasing = false;
        }

        if (chasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Transform targetPoint = patrolPoints[currentPoint];
        Vector2 direction = targetPoint.position - transform.position;
        rb.velocity = new Vector2(Mathf.Sign(direction.x) * patrolSpeed, rb.velocity.y);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = player.position - transform.position;
        rb.velocity = new Vector2(Mathf.Sign(direction.x) * chaseSpeed, rb.velocity.y);

        //maybe put the start of attack() here 
    }
}
