using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [Header("Damage Settings")]
    public int Damage = 10;
    public LayerMask TargetLayerMask;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float upwardBoost = 0.5f;
    public ForceMode2D forceMode = ForceMode2D.Impulse;

    [Header("Parry State")]
    public bool IsParried = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // --- PARRY CHECK ---------------------------------------------
        if (other.CompareTag("Parry"))
        {
            Debug.Log("Enemy attack was parried.");
            IsParried = true;
            return;
        }

        // Check if target can be damaged
        if (!((TargetLayerMask.value & (1 << other.gameObject.layer)) > 0))
            return;

        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth == null)
            return;

        DealDamage(targetHealth, other);
    }

    private void DealDamage(Health targetHealth, Collider2D other)
    {
        targetHealth.Damage(Damage, gameObject);
        Debug.Log("Dealing " + Damage + " damage to " + other.name + ".");
        ApplyKnockback(other);
    }

    private void ApplyKnockback(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (!rb) return;

        Vector2 direction = (other.transform.position - transform.position).normalized;
        direction.y += upwardBoost;
        rb.AddForce(direction * knockbackForce, forceMode);
    }
}



