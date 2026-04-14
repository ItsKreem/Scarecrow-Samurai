using System.Collections;
using UnityEngine;

public class Health : GameManager
{
    public delegate void HitEvent(GameObject source);
    public event HitEvent OnHit;

    public delegate void ResetEvent();
    public event ResetEvent OnHitReset;

    public static event System.Action OnPlayerDeath;

    [Header("Health Settings")]
    public int MaxHealth = 3;
    private int _currentHealth;
    private bool _canDamage = true;
    public HealthBAR HealthBar;

    [Header("Invulnerability")]
    public Cooldown Invulnerability;

    [Header("Effects & Audio")]
    public AudioSource HurtAudio;
    public GameObject hitVFX;
    public GameObject healVFX;

    [Header("Stun Settings")]
    public float stunDuration = 0.5f; // How long the target stays stunned
    private bool isStunned = false;


    [Header("Animation")]
    public string hurtAnimationName = "Hurt"; // Name of the hurt animation state
    public string deathAnimationName = "Death"; // Name of the death animation state
    private Animator animator;

    public float fadeDelay = 0.5f;

    private Rigidbody2D rb;

    public float CurrentHealth => _currentHealth;

    private PlayerMovement playerMovement;

    void Start()
    {
        ResetHealthToMax();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        HandleInvulnerability();
    }

    public void Damage(int damage, GameObject source)
    {
        // ---------------------------------------------
        // PREVENT DAMAGE IF PLAYER IS DASHING
        // ---------------------------------------------
        if (playerMovement != null && playerMovement.IsDashing)
            return;

        if (!_canDamage)
            return;

        _currentHealth -= damage;

        if (CompareTag("Player"))
        {
            HealthBar.SetHealth(_currentHealth);
        }

        // Apply stun if still alive
        if (_currentHealth > 0)
            StartCoroutine(HandleStun());

        // Play hurt animation
        if (animator != null && !string.IsNullOrEmpty(hurtAnimationName))
        {
            animator.Play(hurtAnimationName, -1, 0f);
        }

        // Play hurt sound
        if (HurtAudio != null)
            Instantiate(HurtAudio, transform.position, Quaternion.identity);

        // Spawn hit visual effect
        if (hitVFX != null)
            Instantiate(hitVFX, transform.position, Quaternion.identity);

        // Check for death
        if (_currentHealth <= 0f)
        {
            _currentHealth = 0;
            StartCoroutine(HandleDeath());
        }

        Invulnerability.StartCooldown();
        _canDamage = false;
        OnHit?.Invoke(source);
    }


    private IEnumerator HandleDeath()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        if (CompareTag("Player"))
        {
            Debug.Log("Player has died.");
            OnPlayerDeath?.Invoke();

            // Play death animation (player)
            if (animator != null && !string.IsNullOrEmpty(deathAnimationName))
                animator.Play(deathAnimationName);


            // Fade out
            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeOut();
            }

            // Unlock camera and reset
            CameraRoom.UnlockAndReset();

            if (SavePoint.HasSavePoint())
            {
                yield return StartCoroutine(RespawnPlayer());
            }
            else
            {
                // Fade in before loading main menu
                if (ScreenFader.Instance != null)
                {
                    yield return new WaitForSeconds(fadeDelay);
                    yield return ScreenFader.Instance.FadeIn();
                    MainMenu();
                }
            }
        }
        else
        {
            // Enemy death animation
            if (animator != null && !string.IsNullOrEmpty(deathAnimationName))
            {
                animator.SetBool("Dead", true);
                animator.Play(deathAnimationName);
                // Wait for animation length before destroying
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator HandleStun()
    {
        if (isStunned) yield break;
        isStunned = true;

        // Disable movement
        if (playerMovement != null)
            playerMovement.enabled = false;

        var enemyController = GetComponent<EnemyController>();
        if (enemyController != null)
            enemyController.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        // Re-enable movement
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (enemyController != null)
            enemyController.enabled = true;

        isStunned = false;
    }


    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1f);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (rb != null)
            rb.simulated = true;

        ResetHealthToMax();

        transform.position = SavePoint.GetLastSavePosition();
        Debug.Log($"Player respawned at save point: {SavePoint.GetLastSavePosition()}");

        if (ScreenFader.Instance != null)
        {
            yield return new WaitForSeconds(fadeDelay);
            yield return ScreenFader.Instance.FadeIn();
        }
    }

    public void ResetHealthToMax()
    {
        _currentHealth = MaxHealth;
        _canDamage = true;

        if (CompareTag("Player"))
        {
            if (HealthBar == null)
                HealthBar = FindObjectOfType<HealthBAR>();

            if (HealthBar != null)
                HealthBar.SetMaxHealth(MaxHealth);
        }

        if (healVFX != null)
        {
            Instantiate(healVFX, transform.position, Quaternion.identity, transform);
        }
    }

    private void HandleInvulnerability()
    {
        if (_canDamage)
            return;

        if (Invulnerability.IsOnCooldown)
            return;

        _canDamage = true;
        OnHitReset?.Invoke();
    }
}