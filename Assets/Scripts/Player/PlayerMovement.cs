using System.Collections;
using UnityEngine;

public class PlayerMovement : GameManager
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    private float moveInput;
    private bool isFacingRight = true;
    private bool canMove = true;
    public GameObject groundPoundHitbox;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    private bool isGrounded;

    [Header("Double Jump")]
    private bool canDoubleJump;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashPowerMultiplier = 2f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 1f;
    public bool allowAirDash = true;
    private bool canDash = true;
    private bool isDashing = false;
    public GameObject dashHitbox;
    private float dashCooldownTimer = 0f;
    public bool IsDashing => isDashing;

    [Header("Stomp Settings")]
    public float stompBounceForce = 12f;
    public LayerMask enemyLayer;
    public Transform stompCheck;
    public float stompCheckRadius = 0.25f;

    public Animator animator;
    private Rigidbody2D rb;
    public TrailRenderer trailRenderer;
    public GameObject groundParticles;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();

        //trailRenderer = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!canMove)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetFloat("Speed", 0);
            return;
        }

        moveInput = Input.GetAxis("Horizontal");
        CheckGroundStatus();

        if (!isDashing)
        {
            HandleWalk();
            HandleJump();
        }

        HandleDash();
    }


    // ---------------------------------------------------------
    // GROUND CHECK + DASH RESET
    // ---------------------------------------------------------
    void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsJumping", !isGrounded);

        if (isGrounded)
        {
            groundParticles.SetActive(true);
            canDoubleJump = true;

            // Only refresh dash if cooldown timer has expired
            if (dashCooldownTimer <= 0f)
                canDash = true;
            TurnOffTrailSmooth();
        }
    }


    // ---------------------------------------------------------
    // WALKING
    // ---------------------------------------------------------
    void HandleWalk()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        FlipPlayer();
    }

    // ---------------------------------------------------------
    // JUMP + DOUBLE JUMP
    // ---------------------------------------------------------
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            groundParticles.SetActive(false);
            TurnOnTrail();
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (canDoubleJump)
            {
                animator.SetBool("IsDoubleJumping", false);
                animator.SetBool("IsJumping", false);

                animator.Update(0f);

                animator.SetBool("IsDoubleJumping", true);

                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = false;
            }

        }
    }

    // ---------------------------------------------------------
    // STOMP ON ENEMY HEAD
    // ---------------------------------------------------------
    //void CheckStompEnemy()
    //{
    //    Collider2D enemy = Physics2D.OverlapCircle(stompCheck.position, stompCheckRadius, enemyLayer);

    //    if (enemy != null && rb.velocity.y <= 0)
    //    {
    //        Health enemyHealth = enemy.GetComponent<Health>();
    //        if (enemyHealth != null)
    //            enemyHealth.Damage(1, gameObject);

    //        rb.velocity = new Vector2(rb.velocity.x, stompBounceForce);

    //        // Refresh double jump
    //        canDoubleJump = true;

    //        animator.SetBool("IsDoubleJumping", false);
    //        animator.SetBool("IsJumping", false);
    //        animator.Update(0f);

    //        animator.SetBool("IsDoubleJumping", true);
    //    }
    //}


    // ---------------------------------------------------------
    // MOVEMENT FLIP
    // ---------------------------------------------------------
    void FlipPlayer()
    {
        if (moveInput > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(2, 2, 1);
        }
        else if (moveInput < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-2, 2, 1);
        }
    }

    // ---------------------------------------------------------
    // DASH 
    // ---------------------------------------------------------
    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && (isGrounded || allowAirDash))
        {
            AudioManager.instance.PlaySFX("Dash");
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        TurnOnTrail();
        canDash = false;
        isDashing = true;

        // Start cooldown
        dashCooldownTimer = dashCooldown;

        if (dashHitbox != null)
            dashHitbox.SetActive(true);

        animator.SetBool("IsDashing", true);

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        Vector2 dashDirection =
            Input.GetAxisRaw("Horizontal") != 0 ?
            new Vector2(Input.GetAxisRaw("Horizontal"), 0) :
            (isFacingRight ? Vector2.right : Vector2.left);

        rb.velocity = dashDirection.normalized * dashForce * dashPowerMultiplier;

        yield return new WaitForSeconds(dashDuration);

        if (dashHitbox != null)
            dashHitbox.SetActive(false);

        rb.gravityScale = originalGravity;
        animator.SetBool("IsDashing", false);
        isDashing = false;

        TurnOffTrailSmooth();
    }

    public void TurnOffTrailSmooth()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }
    }

    public void TurnOnTrail()
    {
        if (trailRenderer != null)
        {

            trailRenderer.emitting = true; 
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (stompCheck != null)
            Gizmos.DrawWireSphere(stompCheck.position, stompCheckRadius);
    }
}
