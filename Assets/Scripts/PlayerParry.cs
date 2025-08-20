using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    public GameObject parryField;
    public float parryWindow = 0.3f;
    public KeyCode parryKey = KeyCode.LeftShift;
    public bool IsParrying { get; private set; }

    private float parryStartTime;
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    public Animator animator;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>(); // Reference to your movement script
    }

    void Update()
    {
        if (Input.GetKeyDown(parryKey) && !IsParrying)
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
        animator.SetBool("IsParrying", true);
        parryField.SetActive(true);
        IsParrying = true;
        parryStartTime = Time.time;

        if (playerMovement != null)
            playerMovement.enabled = false; // Lock movement

        rb.velocity = Vector2.zero; // Stop motion
    }

    void EndParry()
    {
        IsParrying = false;
        animator.SetBool("IsParrying", false);
        if (playerMovement != null)
            playerMovement.enabled = true; // Re-enable movement
        parryField.SetActive(false);
    }
}


