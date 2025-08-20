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

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;


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
        moveInput = Input.GetAxis("Horizontal");

        CheckGroundStatus();
        HandleWalk();
        HandleJump();
        HandleScrewAttack();
        HandleDash();
    }

    // ---------------- METHODS ---------------- //

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
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetBool("IsJumping", true);
            }
            else if (canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = false;
                animator.SetBool("IsJumping", true);
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
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            Debug.Log("Shift pressed");
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

        float dashDirection = Mathf.Sign(moveInput != 0 ? moveInput : transform.localScale.x);
        rb.velocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(0.2f); // Dash duration

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

