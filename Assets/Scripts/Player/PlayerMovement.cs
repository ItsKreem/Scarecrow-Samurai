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

    public Animator animator;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {

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

        HandleRestart();
        HandleDash();
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsDoubleJumping", !isGrounded);
        if (isGrounded) canDoubleJump = true;
        groundPoundHitbox.SetActive(false);
    }

    void HandleWalk()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        FlipPlayer();
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                animator.SetBool("IsJumping", true);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (canDoubleJump)
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsDoubleJumping", true);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = false;
            }
        }
    }

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

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void HandleRestart()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenu();
        }
    }
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        if (dashHitbox != null)
            dashHitbox.SetActive(true);

        animator.SetBool("IsDashing", true);

        float originalGravity = rb.gravityScale;
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Detect if player is pressing down (even diagonally)
        bool isGroundPound = inputDir.y < -0.2f;

        if (isGroundPound)
        {
            // Force ground pound straight down
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;

            // Apply downward impulse
            rb.AddForce(Vector2.down * dashForce * dashPowerMultiplier, ForceMode2D.Impulse);

            // Activate ground pound hitbox
            if (groundPoundHitbox != null)
                groundPoundHitbox.SetActive(true);
        }
        else
        {
            // Disable gravity during normal dash
            rb.gravityScale = 0f;

            Vector2 dashDirection = inputDir;
            if (dashDirection == Vector2.zero)
                dashDirection = isFacingRight ? Vector2.right : Vector2.left;

            dashDirection.Normalize();
            rb.velocity = dashDirection * dashForce * dashPowerMultiplier;
        }

        yield return new WaitForSeconds(dashDuration);

        animator.SetBool("IsDashing", false);

        if (dashHitbox != null)
            dashHitbox.SetActive(false);

        // Restore gravity
        rb.gravityScale = originalGravity;
        isDashing = false;

        // Turn off ground pound hitbox after dash ends
        if (groundPoundHitbox != null)
            groundPoundHitbox.SetActive(false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

// --- Save Point Animation ---
public IEnumerator PlaySaveAnimation(float duration)
    {
        canMove = false;
        animator.SetTrigger("IsPraying");
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(duration);
        animator.SetTrigger("NotPraying");

        canMove = true;
    }

    // --- Death Animation ---
    public IEnumerator PlayDeathAnimation()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die"); 

        yield return new WaitForSeconds(1);
    }

    // --- Respawn Animation ---
    public IEnumerator PlayRespawnAnimation()
    {
        animator.SetTrigger("NotPraying"); 
        yield return new WaitForSeconds(1);
        canMove = true;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
