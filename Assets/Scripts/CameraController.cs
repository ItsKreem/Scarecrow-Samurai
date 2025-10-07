using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -10);
    public float smoothSpeed = 5f;

    private bool isLocked = false;
    private Vector3 lockedPosition;

    void LateUpdate()
    {
        if (isLocked)
        {
            // Smoothly move to locked position and stay there
            transform.position = Vector3.Lerp(transform.position, lockedPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            // Follow player with smoothing
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    // Called when colliding with a camera trigger
    private void OnTriggerEnter(Collider other)
    {
        CameraRoom camRoom = other.GetComponent<CameraRoom>();
        if (camRoom != null)
        {
            LockCamera(camRoom.lockPosition.position);
        }
    }

    // Optional: unlock when leaving the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CameraRoom>() != null)
        {
            UnlockCamera();
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
    }
}

