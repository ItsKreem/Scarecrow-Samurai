using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public Vector2 patrolZoneSize = new Vector2(5f, 3f);
    public float waitAtPatrolPoint = 2f;

    [Header("Detection")]
    public Transform player;
    public float detectionRange = 5f;
    public float leapTriggerDistance = 3f;
    public float waitBeforeLeap = 1f;

    [Header("Leap Attack")]
    public float leapForce = 12f;
    public GameObject attackHitbox;
    public float attackHitboxDuration = 0.5f;

    [Header("Parry")]
    public float parryStunDuration = 1.2f;
    public float parryKnockbackForce = 10f;
    public bool isParried = false;

    [Header("Cooldown")]
    public Cooldown leapCooldown;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius = 0.2f;

    [Header("SFX")]
    public GameObject parriedSFX;

    private Rigidbody2D rb;
    private Vector2 spawnPosition;
    private Vector2 patrolTarget;
    private bool chasing = false;
    private bool isLeaping = false;
    private bool waitingAtPatrolPoint = false;
    private float waitTimer = 0f;
    private bool isFacingRight = true; // NEW: track enemy facing direction

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
        ChooseNewPatrolTarget();

        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (attackHitbox != null) attackHitbox.SetActive(false);
        if (leapCooldown == null)
            leapCooldown = new Cooldown { Duration = 3f };
    }

    void Update()
    {
        if (isParried || player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        chasing = distToPlayer <= detectionRange;

        FlipTowardsPlayer();

        if (!isLeaping && !leapCooldown.IsOnCooldown && distToPlayer <= leapTriggerDistance && IsGrounded())
        {
            StartCoroutine(LeapTowardPlayer());
        }
        else if (!isLeaping)
        {
            if (chasing)
                ChasePlayer();
            else
                Patrol();
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

    void Patrol()
    {
        if (waitingAtPatrolPoint)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtPatrolPoint)
            {
                ChooseNewPatrolTarget();
                waitingAtPatrolPoint = false;
            }
            return;
        }

        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(direction.x * patrolSpeed, rb.velocity.y);

        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            waitingAtPatrolPoint = true;
            waitTimer = 0f;
        }
    }

    void ChooseNewPatrolTarget()
    {
        float x = Random.Range(-patrolZoneSize.x / 2f, patrolZoneSize.x / 2f);
        float y = Random.Range(-patrolZoneSize.y / 2f, patrolZoneSize.y / 2f);
        patrolTarget = spawnPosition + new Vector2(x, y);
    }

    void ChasePlayer()
    {
        Vector2 direction = player.position - transform.position;
        rb.velocity = new Vector2(Mathf.Sign(direction.x) * chaseSpeed, rb.velocity.y);
    }

    IEnumerator LeapTowardPlayer()
    {
        isLeaping = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);

        float timer = 0f;
        while (timer < waitBeforeLeap)
        {
            if (isParried) { isLeaping = false; yield break; }
            timer += Time.deltaTime;
            yield return null;
        }

        if (!IsGrounded())
        {
            isLeaping = false;
            yield break;
        }

        Vector2 leapDir = (player.position - transform.position).normalized;
        leapDir.y = 1f;
        rb.AddForce(leapDir * leapForce, ForceMode2D.Impulse);

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            yield return new WaitForSeconds(attackHitboxDuration);
            attackHitbox.SetActive(false);
        }

        leapCooldown.StartCooldown();
        yield return new WaitForSeconds(0.5f);
        isLeaping = false;
    }

    public void OnParried(Vector2 knockbackDir, float customKnockbackForce = -1f)
    {
        if (isParried) return;

        if (parriedSFX != null)
        {
            Instantiate(parriedSFX, transform.position, Quaternion.identity);
        }

        StopAllCoroutines();
        isLeaping = false;

        rb.velocity = Vector2.zero;
        float knockback = customKnockbackForce >= 0f ? customKnockbackForce : parryKnockbackForce;
        rb.AddForce(knockbackDir.normalized * knockback, ForceMode2D.Impulse);
        if (attackHitbox != null) attackHitbox.SetActive(false);

        isParried = true;
        StartCoroutine(RecoverFromParry());
    }


    IEnumerator RecoverFromParry()
    {
        yield return new WaitForSeconds(parryStunDuration);
        isParried = false;
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return true;
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Parry"))
        {
            Vector2 knockbackDir = (transform.position - player.position).normalized;
            OnParried(knockbackDir);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        // Show patrol zone
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Application.isPlaying ? (Vector3)spawnPosition : transform.position, patrolZoneSize);
    }
}


