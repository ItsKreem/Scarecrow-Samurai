using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject attackHitbox;
    public float comboResetTime = 1f; // Time before combo resets
    public KeyCode attackKey = KeyCode.Mouse0;
    public float AttackDuration = 0.15f;

    [Header("Animator")]
    public Animator animator;

    private int comboStep = 0;          // 1, 2, or 3
    private bool isAttacking = false;
    private bool queuedNext = false;
    private float lastAttackTime = 0f;

    [Header("Movement Speed")]
    private PlayerMovement playerMovement;
    private float originalMoveSpeed;
    public float attackSpeedMultiplier = 0.5f; // halves movement speed


    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        // Get movement script
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
            originalMoveSpeed = playerMovement.moveSpeed;
    }


    void Update()
    {
        // avoid hard coding. Make use of the Input or Make use of the new Input System

        if (Input.GetKeyDown(attackKey))
        {
            // Start first attack if idle
            if (!isAttacking)
            {
                comboStep++;
                StartCoroutine(AttackRoutine());
            }
            // Queue next combo step if allowed
            else if (isAttacking)
            {
                queuedNext = true;
            }
        }

        if (comboStep > 3)
        {
            DisableHitbox();
            comboStep = 0;
        }

        // Reset combo if idle too long
        if (!isAttacking && Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        queuedNext = false;
        lastAttackTime = Time.time;

        // Halve player movement speed
        if (playerMovement != null)
            playerMovement.moveSpeed = originalMoveSpeed * attackSpeedMultiplier;

        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
        animator.SetTrigger("Attack" + comboStep);

        EnableHitbox();

        float t = 0f;
        while (t < AttackDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        DisableHitbox();

        // Restore speed after attack ends
        if (playerMovement != null)
            playerMovement.moveSpeed = originalMoveSpeed;

        isAttacking = false;
    }


    // Helper to get the length of an animation clip by name
    private float GetAnimationClipLength(string clipName)
    {
        if (animator.runtimeAnimatorController == null) return 0.5f;
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 0.5f; // fallback
    }

    // --- Called by Animation Events ---
    public void EnableHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }
}


