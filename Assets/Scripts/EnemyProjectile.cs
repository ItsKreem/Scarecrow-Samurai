using UnityEngine;

public class EnemyProjectile : EnemyAttack
{
    public float speed = 5f;
    public float lifeTime = 5f;
    public GameObject parryEffect;
    public LayerMask enemyLayer;

    private Vector2 moveDirection;
    private bool isReflected = false;

    public GameObject player;


    private void Start()
    {

    }
    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health targetHealth = collision.GetComponent<Health>();

        if (!isReflected && collision.CompareTag("Parry"))
        {
            ReflectProjectile();
        }
        else if (!isReflected && collision.CompareTag("Player"))
        {
            Debug.Log("Collided with player");
            HitPlayer(collision);
        }
        else if (isReflected && collision.CompareTag("Enemy"))
        {
            HitEnemy(collision);
        }
        else if(!isReflected && collision.CompareTag("Ground"))
        {
            //put effect here
        }
    }

    void ReflectProjectile()
    {
        isReflected = true;
        speed *= 1.2f; 

        if (parryEffect)
            Instantiate(parryEffect, transform.position, Quaternion.identity);

        // Flip direction
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
        if(((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(1, gameObject);
            }
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