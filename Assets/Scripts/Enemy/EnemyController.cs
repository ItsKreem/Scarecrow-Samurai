using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Detection")]
    public Transform player;

    [Header("Leap Attack")]
    public float leapTriggerDistance = 3f;
    public float waitBeforeLeap = 1f;
    public float leapForce = 12f;
    public GameObject attackHitbox;
    public float attackHitboxDuration = 0.5f;

    [Header("Parry")]
    public float parryStunDuration = 1.2f;
    public float parryKnockbackForce = 10f;
    public bool isParried = false;

    [Header("Cooldown")]
    public Cooldown leapCooldown;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius = 0.2f;

    [Header("SFX")]
    public GameObject parriedSFX;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private bool isLeaping = false;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (leapCooldown == null)
            leapCooldown = new Cooldown { Duration = 3f };
    }

    void Update()
    {
        if (player == null || isParried) return;

        FlipTowardsPlayer();

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // Try leaping if in range
        if (!isLeaping && !leapCooldown.IsOnCooldown && distToPlayer <= leapTriggerDistance && IsGrounded())
        {
            StartCoroutine(LeapTowardPlayer());
            return;
        }

        if (!isLeaping)
            MoveTowardPlayerLogic();
    }

    // ------------------------------
    //     NEW PLAYER-TRACKING LOGIC
    // ------------------------------
    void MoveTowardPlayerLogic()
    {
        Vector2 pos = transform.position;
        Vector2 targetPos = player.position;

        // If player is ABOVE, enemy moves directly underneath player
        if (player.position.y > transform.position.y + 0.2f)
        {
            float directionX = Mathf.Sign(targetPos.x - pos.x);
            rb.velocity = new Vector2(directionX * moveSpeed, rb.velocity.y);
        }
        else
        {
            // Standard chase movement
            float directionX = Mathf.Sign(targetPos.x - pos.x);
            rb.velocity = new Vector2(directionX * moveSpeed, rb.velocity.y);
        }
    }

    void FlipTowardsPlayer()
    {
        float dir = player.position.x - transform.position.x;

        if (dir > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (dir < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    IEnumerator LeapTowardPlayer()
    {
        isLeaping = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);

        float t = 0f;
        while (t < waitBeforeLeap)
        {
            if (isParried) { isLeaping = false; yield break; }
            t += Time.deltaTime;
            yield return null;
        }

        if (!IsGrounded()) { isLeaping = false; yield break; }

        Vector2 leapDir = (player.position - transform.position).normalized;
        leapDir.y = 1f;

        if (animator != null)
            animator.Play("Enemy_Attack");

        rb.AddForce(leapDir * leapForce, ForceMode2D.Impulse);

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            yield return new WaitForSeconds(attackHitboxDuration);
            attackHitbox.SetActive(false);
        }

        while (!IsGrounded())
        {
            if (isParried) { isLeaping = false; yield break; }
            yield return null;
        }

        if (animator != null)
            animator.Play("Enemy_Walk");

        leapCooldown.StartCooldown();
        yield return new WaitForSeconds(2f);

        isLeaping = false;
    }

    public void OnParried(Vector2 knockbackDir, float customKnockbackForce = -1f)
    {

        if (parriedSFX != null)
            Instantiate(parriedSFX, transform.position, Quaternion.identity);

        StopAllCoroutines();
        isLeaping = false;

        rb.velocity = Vector2.zero;

        float knock = customKnockbackForce >= 0 ? customKnockbackForce : parryKnockbackForce;
        rb.AddForce(knockbackDir.normalized * knock, ForceMode2D.Impulse);

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        isParried = true;

        StartCoroutine(RecoverFromParry());
    }

    IEnumerator RecoverFromParry()
    {
        float t = 0f;
        while (t < parryStunDuration)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            t += Time.deltaTime;
            yield return null;
        }
        isParried = false;
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return true;
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Parry"))
        {
            Vector2 knockDir = (transform.position - player.position).normalized;
            OnParried(knockDir);
        }
    }
}