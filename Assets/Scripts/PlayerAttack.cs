using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("PlayerAttack")]
    public GameObject attackHitbox;
    public float attackHitboxDuration = 0.5f;
    public float attackCooldown = 1f; // Cooldown time in seconds
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Animator")]
    public Animator animator;

    private bool canAttack = true;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (attackHitbox != null)
            attackHitbox.SetActive(false); // Ensure hitbox starts inactive
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey) && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        animator.SetBool("IsAttacking", true);

        if (attackHitbox != null)
            attackHitbox.SetActive(true);

        yield return new WaitForSeconds(attackHitboxDuration);

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        yield return new WaitForSeconds(attackCooldown); // Wait for cooldown duration
        canAttack = true;

        animator.SetBool("IsAttacking", false);
    }
}


