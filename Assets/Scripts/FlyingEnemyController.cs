using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    public Transform[] patrolPoints; // Assign in inspector
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float waitTimeAtPoint = 2f;
    public float detectionRadius = 6f;
    public LayerMask playerLayer;
    public float stopDistance = 4f;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootInterval = 2f;

    private Transform player;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private float shootTimer = 0f;
    private bool playerDetected = false;

    void Start()
    {
    }

    void Update()
    {
        DetectPlayer();

        if (playerDetected && player != null)
        {
            ChasePlayer();
            HandleShooting();
        }
        else
        {
            Patrol();
        }
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (hit)
        {
            playerDetected = true;
            player = hit.transform;
        }
        else
        {
            playerDetected = false;
            player = null;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                waitTimer = 0f;
            }
        }
    }

    void ChasePlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)(direction * chaseSpeed * Time.deltaTime);
        }
    }

    void HandleShooting()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            ShootProjectile();
            shootTimer = 0f;
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab != null && shootPoint != null && player != null)
        {
            GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Vector2 dir = (player.position - shootPoint.position).normalized;
            proj.GetComponent<EnemyProjectile>().Initialize(dir);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
