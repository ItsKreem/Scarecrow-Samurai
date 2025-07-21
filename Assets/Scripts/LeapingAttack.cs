using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyLeapAttack : MonoBehaviour
{
    [Header("Detection & Leap")]
    public float leapTriggerDistance = 3f;
    public float waitBeforeLeap = 1f;
    public float leapForce = 12f;

    [Header("Parry")]
    public float parryStunDuration = 1.2f;
    public bool isParried = false;

    [Header("Cooldown")]
    public Cooldown leapCooldown;

    [Header("References")]
    public Transform player;
    public GameObject attackHitbox;

    private Rigidbody2D rb;
    private bool isLeaping = false;

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
        if (player == null || isLeaping || isParried || leapCooldown.IsOnCooldown) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= leapTriggerDistance)
        {
            StartCoroutine(LeapTowardPlayer());
        }
    }

    IEnumerator LeapTowardPlayer()
    {
        isLeaping = true;

        // Freeze enemy and wait
        rb.velocity = Vector2.zero;

        float timer = 0f;
        while (timer < waitBeforeLeap)
        {
            // If parried mid-wait, cancel leap
            if (isParried)
            {
                isLeaping = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Final check before leap
        if (isParried)
        {
            isLeaping = false;
            yield break;
        }

        // Apply leap force
        Vector2 dir = (player.position - transform.position).normalized;
        rb.AddForce(dir * leapForce, ForceMode2D.Impulse);

        // Activate hitbox for short time
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
}