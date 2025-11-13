using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    [Header("Movement & Patrol")]
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 2f;
    [Tooltip("How far from spawn the enemy can patrol (X = horizontal range, Y = vertical range)")]
    public Vector2 patrolRange = new Vector2(10f, 8f);

    [Header("Chase & Combat")]
    public float chaseSpeed = 3.5f;
    public float detectionRadius = 6f;
    public float stopDistance = 4f;
    public LayerMask playerLayer;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootInterval = 2f;

    [Header("Ground Avoidance")]
    public float groundCheckDistance = 20f;   
    public float minHeightAboveGround = 2f;   
    public LayerMask groundLayer;             

    private Transform player;
    private Vector2 spawnPosition;
    private Vector2 patrolTarget;
    private float waitTimer = 0f;
    private float shootTimer = 0f;
    private bool playerDetected = false;
    private bool isFacingRight = true;

    private Animator anim;

    void Start()
    {
        spawnPosition = transform.position;
        ChooseNewPatrolTarget();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        DetectPlayer();

        if (playerDetected && player != null)
        {
            FlipTowardsPlayer(); 
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
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                ChooseNewPatrolTarget();
                waitTimer = 0f;
            }
        }
        else
        {
            Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(direction * patrolSpeed * Time.deltaTime);

            // Flip visually based on patrol target
            if (direction.x > 0 && !isFacingRight)
            {
                isFacingRight = true;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x < 0 && isFacingRight)
            {
                isFacingRight = false;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }


    void ChooseNewPatrolTarget()
    {
        Vector2 newTarget;
        do
        {
            float xOffset = Random.Range(-patrolRange.x, patrolRange.x);
            float yOffset = Random.Range(-patrolRange.y, patrolRange.y);
            newTarget = spawnPosition + new Vector2(xOffset, yOffset);

            RaycastHit2D hit = Physics2D.Raycast(newTarget, Vector2.down, groundCheckDistance, groundLayer);
            if (hit.collider != null)
            {
                float groundY = hit.point.y;
                if (newTarget.y < groundY + minHeightAboveGround)
                {
                    newTarget.y = groundY + minHeightAboveGround;
                }
            }

        } while (Vector2.Distance(newTarget, transform.position) < 1f); // avoid too-close patrol points

        patrolTarget = newTarget;
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
            if (anim != null)
                anim.Play("FlyingEnemy_Shoot");
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

    void FlipTowardsPlayer()
    {
        if (player == null) return;

        float directionToPlayer = player.position.x - transform.position.x;

        if (directionToPlayer > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionToPlayer < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = Application.isPlaying ? spawnPosition : transform.position;
        Vector3 size = new Vector3(patrolRange.x * 2f, patrolRange.y * 2f, 1f);
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}