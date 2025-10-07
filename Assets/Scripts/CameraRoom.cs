using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraRoom : MonoBehaviour
{
    [Header("Lock Position")]
    public Transform lockPosition;

    [Header("Walls")]
    public GameObject LeftWall;
    public GameObject RightWall;

    [Header("Wave Spawner")]
    public GameObject WaveSpawner;
    public WaveSpawner waveSpawner; 

    [Header("Player Settings")]
    public string playerTag = "Player";

    private CameraController camController;

    void Awake()
    {
        if (Camera.main != null)
            camController = Camera.main.GetComponent<CameraController>();

        if (camController == null)
            camController = FindObjectOfType<CameraController>();

        if (camController == null)
            Debug.LogWarning("CameraRoom: No CameraController found. Attach CameraController to your camera.");

        if (lockPosition == null)
            lockPosition = transform;

        if (waveSpawner != null)
        {
            // Subscribe to wave completion event
            waveSpawner.OnAllWavesCompleted += HandleWavesCompleted;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (camController != null)
            camController.LockCamera(lockPosition.position);

        if (LeftWall != null) LeftWall.SetActive(true);
        if (RightWall != null) RightWall.SetActive(true);
        if (WaveSpawner != null) WaveSpawner.SetActive(true);

        if (waveSpawner != null)
            waveSpawner.BeginSpawning(); // tell spawner to start

        Debug.Log($"CameraRoom: Player entered, locking camera and starting waves at {lockPosition.position}");
    }

    private void HandleWavesCompleted()
    {
        if (camController != null)
            camController.UnlockCamera();

        if (LeftWall != null) LeftWall.SetActive(false);
        if (RightWall != null) RightWall.SetActive(false);

        Debug.Log("CameraRoom: Waves finished. Unlocking camera and deactivating walls.");
    }

    private void OnDrawGizmos()
    {
        if (lockPosition != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(lockPosition.position, 0.25f);
            Gizmos.DrawLine(transform.position, lockPosition.position);
        }
    }
}




