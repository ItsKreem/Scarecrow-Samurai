using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    private float moveInput;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    private bool isGrounded;

    [Header("Double Jump")]
    private bool canDoubleJump;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Screw Attack")]
    public bool isScrewAttacking = false;
    public float screwAttackDuration = 0.4f;
    public GameObject screwAttackHitbox;

    public Animator animator;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        if (!isDashing)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset double jump when grounded
        if (isGrounded)
        {;
            canDoubleJump = true;
            animator.SetBool("IsJumping", false);
        }

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetBool("IsJumping", true);
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = false;
            }
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // Screw Attack
        if (Input.GetKeyDown(KeyCode.Space) && !isGrounded)
        {
            StartCoroutine(ScrewAttack());
        }
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

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
