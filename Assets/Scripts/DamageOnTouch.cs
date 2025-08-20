using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public delegate void OnHitSomething();

    public OnHitSomething OnHit;

    [Header("Damage Settings")]
    public float Damage = 10f;
    public LayerMask TargetLayerMask;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public ForceMode2D forceMode = ForceMode2D.Impulse;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // You can use this if you need collision-based damage.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Successfully collided with " + other + ".");

        if (!((TargetLayerMask.value & (1 << other.gameObject.layer)) > 0))
            return;

        Health targetHealth = other.gameObject.GetComponent<Health>();
        if (targetHealth == null)
            return;

        TryDamage(targetHealth, other);
    }

    private void TryDamage(Health targetHealth, Collider2D other)
    {
        targetHealth.Damage(Damage, transform.parent != null ? transform.parent.gameObject : gameObject);
        Debug.Log("Hit " + targetHealth);
        ApplyKnockback(other);
        OnHit?.Invoke();
    }

    private void ApplyKnockback(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            rb.AddForce(direction * knockbackForce, forceMode);
        }
    }
}

