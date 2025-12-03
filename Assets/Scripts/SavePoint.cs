using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [Header("Settings")]
    public bool isActivated = false;
    public float saveAnimationDuration = 2f; // <--- adjustable duration

    private static Vector3 lastSavePosition;
    private static bool hasSavePoint = false;
    public Animator animator;

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



        // ✅ Reset player health to max
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            // Make sure it updates the health bar too
            playerHealth.ResetHealthToMax();
            if (playerHealth.HealthBar != null)
                playerHealth.HealthBar.SetHealth(playerHealth.MaxHealth);
        }

        Debug.Log("Save point activated at " + lastSavePosition);

        // You can also add particle effects, sound, etc. here
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
