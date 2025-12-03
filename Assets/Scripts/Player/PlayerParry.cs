using System.Collections;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry Settings")]
    public GameObject parryField;
    public float parryWindow = 1f;
    public float parryCooldown = 1.5f;  
    public KeyCode parryKey = KeyCode.LeftShift;

    public bool IsParrying { get; private set; }
    public bool IsOnCooldown { get; private set; }

    private float parryStartTime;
    private float lastParryTime;         
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    public Animator animator;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Check cooldown
        if (IsOnCooldown && Time.time > lastParryTime + parryCooldown)
            IsOnCooldown = false;

        // Start parry only if NOT parrying and NOT on cooldown
        if (Input.GetKeyDown(parryKey) && !IsParrying && !IsOnCooldown)
        {
            StartParry();
        }

        // End parry after window
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
        IsOnCooldown = true;
        lastParryTime = Time.time; 

        animator.SetBool("IsParrying", false);
        parryField.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true; // Restore movement
    }
}



