using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    public float parryWindow = 0.3f;
    public KeyCode parryKey = KeyCode.Mouse1;

    private float parryStartTime;
    public bool IsParrying { get; private set; }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(parryKey))
        {
            StartParry();
        }

        if (IsParrying && Time.time > parryStartTime + parryWindow)
        {
            EndParry();
        }
    }

    void StartParry()
    {
        IsParrying = true;
        parryStartTime = Time.time;
        //GetComponent<EnemyKnockback>().Knockback();
        //GetComponent<Health>().TakeDamage(1);
    }

    void EndParry()
    {
        IsParrying = false;
    }
}

