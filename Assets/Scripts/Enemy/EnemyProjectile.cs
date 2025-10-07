using UnityEngine;

public class EnemyProjectile : EnemyAttack
{
    public float speed = 5f;
    public float lifeTime = 5f;
    public GameObject parryEffect;
    public LayerMask enemyLayer;
    public LayerMask groundLayer; // Reference for ground layer

    private Vector2 moveDirection;
    private bool isReflected = false;

    public GameObject player;

    [Header("SFX")]
    public GameObject parrySFX;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ? Detect ground by layer OR tag
        if (((1 << collision.gameObject.layer) & groundLayer.value) != 0 || collision.CompareTag("Ground"))
        {
            Debug.Log("Projectile hit ground, destroying.");
            Destroy(gameObject);
            return;
        }

        // Handle parry
        if (!isReflected && collision.CompareTag("Parry"))
        {
            ReflectProjectile();
            return;
        }

        // Handle hitting player
        if (!isReflected && collision.CompareTag("Player"))
        {
            HitPlayer(collision);
            return;
        }

        // Handle hitting enemy after reflection
        if (isReflected && collision.CompareTag("Enemy"))
        {
            HitEnemy(collision);
        }
    }


    void ReflectProjectile()
    {
        isReflected = true;
        speed *= 1.2f;

        if (parryEffect)
            Instantiate(parryEffect, transform.position, Quaternion.identity);

        if (parrySFX != null)
            Instantiate(parrySFX, transform.position, Quaternion.identity);

        // Flip direction toward nearest enemy
        GameObject enemy = FindClosestEnemy();
        if (enemy != null)
        {
            Vector2 reflectDir = (enemy.transform.position - transform.position).normalized;
            moveDirection = reflectDir;
        }
        else
        {
            moveDirection = -moveDirection;
        }
    }

    public void HitPlayer(Collider2D other)
    {
        Health playerHealth = other.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.Damage(1, gameObject);
            Destroy(gameObject);
        }
    }

    public void HitEnemy(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.Damage(3, gameObject);
                Destroy(gameObject);
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 10f, enemyLayer);
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.gameObject;
            }
        }

        return closest;
    }
}


