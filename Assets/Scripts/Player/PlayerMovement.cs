using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    private float moveInput;
    private bool isFacingRight = true;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    private bool isGrounded;

    [Header("Double Jump")]
    private bool canDoubleJump;

    [Header("Screw Attack")]
    public bool isScrewAttacking = false;
    public float screwAttackDuration = 0.4f;
    public GameObject screwAttackHitbox;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashPowerMultiplier = 2f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 1f;
    public bool allowAirDash = true; // toggle air dash on/off
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
        Cursor.visible = false;
        moveInput = Input.GetAxis("Horizontal");

        CheckGroundStatus();

        if (!isDashing) // Disable normal movement during dash
        {
            HandleWalk();
            HandleJump();
        }

        HandleScrewAttack();
        HandleDash();
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            canDoubleJump = true;
            animator.SetBool("IsJumping", false);
        }
    }

    void HandleWalk()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        FlipPlayer();
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && !isScrewAttacking)
        {
            if (isGrounded)
            {
                animator.SetBool("IsJumping", true);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (canDoubleJump)
            {
                animator.SetBool("IsJumping", true);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = false;
            }
        }
    }

    void HandleScrewAttack()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isGrounded && !isScrewAttacking)
        {
            StartCoroutine(ScrewAttack());
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

    IEnumerator ScrewAttack()
    {
        isScrewAttacking = true;

        if (screwAttackHitbox != null)
            screwAttackHitbox.SetActive(true);

        yield return new WaitForSeconds(screwAttackDuration);

        if (screwAttackHitbox != null)
            screwAttackHitbox.SetActive(false);

        isScrewAttacking = false;
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // Enable dash hitbox
        if (dashHitbox != null)
            dashHitbox.SetActive(true);

        animator.SetBool("IsDashing", true); //ani start

        Vector2 dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (dashDirection == Vector2.zero)
            dashDirection = isFacingRight ? Vector2.right : Vector2.left;

        dashDirection.Normalize();
        rb.velocity = dashDirection * dashForce * dashPowerMultiplier;

        yield return new WaitForSeconds(dashDuration);

        animator.SetBool("IsDashing", false);

        // Disable dash hitbox after dash ends
        if (dashHitbox != null)
            dashHitbox.SetActive(false);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}


