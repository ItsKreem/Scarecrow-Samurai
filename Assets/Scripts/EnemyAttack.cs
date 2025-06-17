using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyKnockback
{
    public float attackCooldown = 2f;
    public float attackRange = 1.2f;
    public int damage = 1;
    public LayerMask playerLayer;
    public GameObject parryLight;
    //public float knockbackForce = 50;

    private float lastAttackTime;
    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= attackRange)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        parryLight.SetActive(true);
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hit != null)
            {
                PlayerParry parry = hit.GetComponent<PlayerParry>();

                if (parry != null && parry.IsParrying)
                {
                    // Parry successful
                    StunAndKnockback();
                    parryLight.SetActive(false);
                }
                else
                {
                    // Deal damage
                    Health playerHealth = hit.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damage);
                        KnockbackPlayer(hit.transform);
                        StartCoroutine(PauseAfterHit());
                    }
                }
            }
    }

    void StunAndKnockback()
    {
        // Apply knockback force and disable movement temporarily
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 knockbackDir = (transform.position.x > player.position.x) ? Vector2.right : Vector2.left;
        rb.AddForce(knockbackDir * 300f); // Adjust force as needed

        StartCoroutine(StunCoroutine());
    }

    IEnumerator StunCoroutine()
    {
        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.enabled = false;
            yield return new WaitForSeconds(1.2f); // Stun duration
            controller.enabled = true;
        }
    }

    void KnockbackPlayer(Transform player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockDir = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;
            playerRb.velocity = Vector2.zero; // reset before knock
            playerRb.AddForce(((5f * knockDir) + Vector2.up * 1f) * 350f); 
        }
    }

    IEnumerator PauseAfterHit()
    {
        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.enabled = false;
            yield return new WaitForSeconds(2f); // Pause duration
            controller.enabled = true;
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}