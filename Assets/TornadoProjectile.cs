using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoProjectile : MonoBehaviour
{
    [Header("Fall")]
    public float fallSpeed = 8f;

    [Header("Ground Patrol")]
    public float moveSpeed = 4f;
    public float patrolRange = 3f;        // Total horizontal range
    public float returnToLineSpeed = 10f; // How fast it snaps back to Y

    [Header("Lifetime")]
    public float lifeTime = 4f;

    private Rigidbody2D rb;
    private bool hasHitGround;

    private float patrolYLevel;
    private float pointAX;
    private float pointBX;
    private float targetX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void Start()
    {
        rb.velocity = Vector2.down * fallSpeed;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!hasHitGround)
            return;

        HandleMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasHitGround && collision.collider.CompareTag("Ground"))
        {
            hasHitGround = true;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;

            patrolYLevel = transform.position.y;

            float centerX = transform.position.x;
            pointAX = centerX - patrolRange * 0.5f;
            pointBX = centerX + patrolRange * 0.5f;

            targetX = Random.value > 0.5f ? pointAX : pointBX;
        }
    }

    void HandleMovement()
    {
        Vector3 pos = transform.position;

        // Snap back to patrol Y line
        if (Mathf.Abs(pos.y - patrolYLevel) > 0.05f)
        {
            pos.y = Mathf.Lerp(pos.y, patrolYLevel, Time.deltaTime * returnToLineSpeed);
            transform.position = pos;
            return;
        }

        // Move between Point A and Point B
        Vector3 targetPos = new Vector3(targetX, patrolYLevel, pos.z);
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // Switch direction
        if (Mathf.Abs(transform.position.x - targetX) < 0.05f)
        {
            targetX = Mathf.Approximately(targetX, pointAX)
                ? pointBX
                : pointAX;
        }
    }
}
