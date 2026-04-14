using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [Header("Settings")]
    public bool isActivated = false;
    public float saveAnimationDuration = 2f; // <--- adjustable duration

    private static Vector3 lastSavePosition;
    private static bool hasSavePoint = false;
    public Animator animator;

    [Header("SFX")]
    public GameObject healParticles;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ActivateSavePoint(collision.gameObject);
        }
    }

    void ActivateSavePoint(GameObject player)
    {
        if (isActivated) return; // prevent spamming
        animator.Play("StatueAnimation");
        isActivated = true;
        lastSavePosition = transform.position;
        hasSavePoint = true;

        if (healParticles != null)
        {
            GameObject particles = Instantiate(healParticles, transform.position, Quaternion.identity);

            Debug.Log("Particles spawned!");

            var ps = particles.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            else
            {
                Debug.LogWarning("No ParticleSystem component found!");
            }

            Destroy(particles, 5f);
        }

        //Reset player health to max
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            // Make sure it updates the health bar too
            playerHealth.ResetHealthToMax();
            if (playerHealth.HealthBar != null)
                playerHealth.HealthBar.SetHealth(playerHealth.MaxHealth);
        }

        Debug.Log("Save point activated at " + lastSavePosition);
    }

    public static Vector3 GetLastSavePosition()
    {
        return hasSavePoint ? lastSavePosition : Vector3.zero;
    }

    public static bool HasSavePoint()
    {
        return hasSavePoint;
    }
}
