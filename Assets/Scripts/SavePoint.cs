using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [Header("Settings")]
    public bool isActivated = false;

    private static Vector3 lastSavePosition;
    private static bool hasSavePoint = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            ActivateSavePoint(collision.gameObject);
        }
    }

    void ActivateSavePoint(GameObject player)
    {
        // Record position
        lastSavePosition = transform.position;
        hasSavePoint = true;
        isActivated = true;

        Debug.Log("Save point activated at " + lastSavePosition);
    }

    // Called by the player's Health script when they die
    public static Vector3 GetLastSavePosition()
    {
        return hasSavePoint ? lastSavePosition : Vector3.zero;
    }

    public static bool HasSavePoint()
    {
        return hasSavePoint;
    }
}

