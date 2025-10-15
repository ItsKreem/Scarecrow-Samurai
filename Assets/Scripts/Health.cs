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
    public float MaxHealth = 3f;
    private float _currentHealth;
    private bool _canDamage = true;

    [Header("Invulnerability")]
    public Cooldown Invulnerability;

    [Header("Effects & Audio")]
    public AudioSource HurtAudio;
    public GameObject hitVFX;

    public float fadeDelay = 0.5f; // Small pause before fade

    public float CurrentHealth => _currentHealth;

    void Start()
    {
        ResetHealthToMax();
    }

    void Update()
    {
        HandleInvulnerability();
    }

    public void Damage(float damage, GameObject source)
    {
        if (!_canDamage)
            return;

        _currentHealth -= damage;

        if (HurtAudio != null)
            Instantiate(HurtAudio, transform.position, Quaternion.identity);

        if (hitVFX != null)
            Instantiate(hitVFX, transform.position, Quaternion.identity);

        if (_currentHealth <= 0f)
        {
            _currentHealth = 0f;
            StartCoroutine(HandleDeath());
        }

        Invulnerability.StartCooldown();
        _canDamage = false;
        OnHit?.Invoke(source);
    }

    private IEnumerator HandleDeath()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("Player has died.");
            OnPlayerDeath?.Invoke();

            // Fade to black before respawn
            if (ScreenFader.Instance != null)
            {
                yield return new WaitForSeconds(fadeDelay);
                yield return ScreenFader.Instance.FadeOut();
            }

            // Unlock camera and reset room
            CameraRoom.UnlockAndReset();

            if (SavePoint.HasSavePoint())
            {
                yield return StartCoroutine(RespawnPlayer());
            }
            else
            {
                MainMenu();
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1f);

        ResetHealthToMax();
        transform.position = SavePoint.GetLastSavePosition();

        Debug.Log($"Player respawned at save point: {SavePoint.GetLastSavePosition()}");

        // Fade back in
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeIn();
        }
    }

    private void ResetHealthToMax()
    {
        _currentHealth = MaxHealth;
        _canDamage = true;
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


