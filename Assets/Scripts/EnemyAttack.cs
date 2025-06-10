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
                parryLight.SetActive(true);
                
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hit != null)
        {
            PlayerParry parry = hit.GetComponent<PlayerParry>();
            if (parry != null && parry.IsParrying)
            {
                // Enemy is parried!
                GetComponent<Health>().TakeDamage(1);
            }
            else
            {
                hit.GetComponent<Health>().TakeDamage(damage);
                //GetComponent<EnemyKnockback>().Knockback(, knockbackForce);
                parryLight.SetActive(false);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}