using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [Header("Wall Settings")]
    public GameObject breakEffect;
    public float dashBreakBoostMultiplier = 1.2f;

    [Header("Tags")]
    public string attackHitboxTag = "Attack";
    public string dashHitboxTag = "Dash";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(attackHitboxTag) || other.CompareTag(dashHitboxTag))
        {
            Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
            if (rb != null && other.CompareTag(dashHitboxTag))
            {
                rb.velocity = rb.velocity * dashBreakBoostMultiplier;
            }

            DestroyWall();
        }
    }

    void DestroyWall()
    {
        if (breakEffect != null)
            Instantiate(breakEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}