using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject attackHitbox;
    public float comboResetTime = 1f; // Time before combo resets
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Animator")]
    public Animator animator;

    private int comboStep = 0;          // 1, 2, or 3
    private bool isAttacking = false;
    private bool queuedNext = false;
    private float lastAttackTime = 0f;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            // Start first attack if idle
            if (!isAttacking)
            {
                comboStep = 1;
                StartCoroutine(AttackRoutine());
            }
            // Queue next combo step if allowed
            else if (isAttacking)
            {
                queuedNext = true;
            }
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

        // Trigger the current combo attack animation
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
        animator.SetTrigger("Attack" + comboStep);

        // --- Wait until current animation nears end ---
        float attackDuration = GetAnimationClipLength("Attack" + comboStep);
        float comboWindow = attackDuration * 0.6f; // when next input can be queued

        yield return new WaitForSeconds(comboWindow);

        // Wait to see if player pressed again during combo window
        float remaining = attackDuration - comboWindow;
        float t = 0f;
        while (t < remaining)
        {
            if (queuedNext && comboStep < 3)
            {
                comboStep++;
                queuedNext = false;
                // Start next attack
                StartCoroutine(AttackRoutine());
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        // Attack finished, reset states
        isAttacking = false;
        queuedNext = false;
        comboStep = 0;
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




