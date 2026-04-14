using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraRoom : MonoBehaviour
{
    [Header("Lock Position")]
    public Transform lockPosition;

    [Header("Camera Zoom")]
    public bool overrideZoom = false;
    public float roomOrthoSize = 8f;

    [Header("Walls")]
    public GameObject LeftWall;
    public GameObject RightWall;

    [Header("Wave Spawner")]
    public GameObject WaveSpawner;
    public WaveSpawner waveSpawner;

    [Header("Player Settings")]
    public string playerTag = "Player";

    private CameraController camController;

    public static CameraRoom ActiveRoom { get; private set; }

    void Awake()
    {
        camController = FindObjectOfType<CameraController>();

        if (lockPosition == null)
            lockPosition = transform;

        if (waveSpawner != null)
            waveSpawner.OnAllWavesCompleted += HandleWavesCompleted;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        ActiveRoom = this;

        if (camController != null)
        {
            camController.LockCamera(lockPosition.position);

            // 👇 Apply zoom if enabled
            if (overrideZoom)
                camController.SetZoom(roomOrthoSize);
        }

        if (LeftWall != null) LeftWall.SetActive(true);
        if (RightWall != null) RightWall.SetActive(true);
        if (WaveSpawner != null) WaveSpawner.SetActive(true);

        if (waveSpawner != null)
            waveSpawner.BeginSpawning();
    }

    private void HandleWavesCompleted()
    {
        if (camController != null)
            camController.UnlockCamera();

        if (LeftWall != null) LeftWall.SetActive(false);
        if (RightWall != null) RightWall.SetActive(false);
    }

    public void ResetRoom()
    {
        if (camController != null)
            camController.UnlockCamera();

        if (LeftWall != null) LeftWall.SetActive(false);
        if (RightWall != null) RightWall.SetActive(false);

        if (waveSpawner != null)
        {
            waveSpawner.StopAllCoroutines();
            waveSpawner.ResetRoom();
        }

        if (WaveSpawner != null)
            WaveSpawner.SetActive(false);
    }

    public static void UnlockAndReset()
    {
        if (ActiveRoom != null)
        {
            ActiveRoom.ResetRoom();
            ActiveRoom = null;
        }
    }
}
