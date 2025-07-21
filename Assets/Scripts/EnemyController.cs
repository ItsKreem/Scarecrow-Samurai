using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public Transform[] patrolPoints;
    private int currentPoint = 0;

    [Header("Detection")]
    public Transform player;
    public float detectionRange = 5f;
    public float leapTriggerDistance = 3f;
    public float waitBeforeLeap = 1f;

    [Header("Leap Attack")]
    public float leapForce = 12f;
    public GameObject attackHitbox;

    [Header("Parry")]
    public float parryStunDuration = 1.2f;
    public bool isParried = false;

    [Header("Cooldown")]
    public Cooldown leapCooldown;

    private bool isLeaping = false;
    private bool chasing = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (attackHitbox != null) attackHitbox.SetActive(false);

        if (leapCooldown == null)
            leapCooldown = new Cooldown { Duration = 3f };
    }

    void Update()
    {
        if (isParried || isLeaping || leapCooldown.IsOnCooldown)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= detectionRange)
        {
            chasing = true;
        }
        else if (distToPlayer > detectionRange + 1f)
        {
            chasing = false;
        }

        if (distToPlayer <= leapTriggerDistance)
        {
            StartCoroutine(LeapTowardPlayer());
        }
        else if (chasing)
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
    }

    IEnumerator LeapTowardPlayer()
    {
        isLeaping = true;
        rb.velocity = Vector2.zero;

        float timer = 0f;
        while (timer < waitBeforeLeap)
        {
            if (isParried)
            {
                isLeaping = false;
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (isParried)
        {
            isLeaping = false;
            yield break;
        }

        Vector2 leapDir = (player.position - transform.position).normalized;
        rb.AddForce(leapDir * leapForce, ForceMode2D.Impulse);

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            attackHitbox.SetActive(false);
        }

        leapCooldown.StartCooldown();
        isLeaping = false;
    }

    public void OnParried(Vector2 knockbackDir, float knockbackForce)
    {
        if (isParried) return;

        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        isParried = true;
        StartCoroutine(RecoverFromParry());
    }

    IEnumerator RecoverFromParry()
    {
        yield return new WaitForSeconds(parryStunDuration);
        isParried = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Called when enemy collides with the player's ParryField
        if (collision.gameObject.CompareTag("Parry"))
        {
            Vector2 knockbackDir = (transform.position - player.position).normalized;
            OnParried(knockbackDir, 10f); // Adjust knockback force if needed
        }
    }
}
