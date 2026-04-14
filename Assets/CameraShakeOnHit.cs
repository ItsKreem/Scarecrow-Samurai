using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeOnHit : MonoBehaviour
{
    public CameraShaker shaker;
    public Health playerHealth;

    void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnHit += HandleHit;
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHit -= HandleHit;
    }

    void HandleHit(GameObject source)
    {
        if (shaker != null)
            shaker.TriggerShake();
    }
}
