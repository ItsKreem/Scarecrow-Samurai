using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -10);
    public float smoothSpeed = 5f;

    [Header("Zoom Settings")]
    public float zoomSmoothSpeed = 5f;

    private bool isLocked = false;
    private Vector3 lockedPosition;

    private Camera cam;
    private float defaultOrthoSize;
    private float targetOrthoSize;
    private bool isZooming = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null && cam.orthographic)
        {
            defaultOrthoSize = cam.orthographicSize;
            targetOrthoSize = defaultOrthoSize;
        }
    }

    void LateUpdate()
    {
        // Position logic
        if (isLocked)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                lockedPosition,
                smoothSpeed * Time.deltaTime
            );
        }
        else
        {
            Vector3 desiredPosition = player.position + offset;
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );
        }

        // Zoom logic
        if (cam != null && cam.orthographic && isZooming)
        {
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                targetOrthoSize,
                zoomSmoothSpeed * Time.deltaTime
            );
        }
    }

    public void LockCamera(Vector3 position)
    {
        isLocked = true;
        lockedPosition = position;
    }

    public void UnlockCamera()
    {
        isLocked = false;
        ResetZoom();
    }

    public void SetZoom(float newSize)
    {
        if (cam == null || !cam.orthographic) return;

        targetOrthoSize = newSize;
        isZooming = true;
    }

    public void ResetZoom()
    {
        if (cam == null || !cam.orthographic) return;

        targetOrthoSize = defaultOrthoSize;
        isZooming = true;
    }
}

