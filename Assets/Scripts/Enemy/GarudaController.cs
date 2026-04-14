using System;
using UnityEngine;

public class GarudaController : MonoBehaviour
{
    public enum BossState
    {
        Shooting,
        DivePause,
        Diving,
        Returning
    }

    [Header("State Durations")]
    public float shootingDuration = 4f;
    public float divePauseDuration = 1f;
    public float maxDiveDuration = 3f;
    public float maxReturnDuration = 2f;

    private float stateTimer;
    public BossState currentState;

    [Header("Patrol")]
    public float moveSpeed = 3f;
    public float patrolYLevel;
    public float pointAX = -6f;
    public float pointBX = 6f;
    private float targetX;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float shootInterval = 2f;
    public Transform shootPoint;
    private float shootTimer;

    [Header("Dive")]
    public float diveSpeed = 1f;
    public float returnSpeed = 1f;
    public GameObject damageHitbox;

    public float groundYLevel = -4f;    // Where the boss hits the ground

    private Vector3 diveStartPos;
    private Vector3 diveEndPos;
    private float diveLerpTime;
    private float returnLerpTime;

    private Vector3 diveTarget;
    public Transform player;
    private bool isFacingRight = true;
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    public TrailRenderer trailRenderer;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        patrolYLevel = transform.position.y;
        targetX = pointBX;
        spriteRenderer = GetComponent<SpriteRenderer>();

        ChangeState(BossState.Shooting);
    }

    void Update()
    {
        if (player == null) return;

        FlipTowardsPlayer();

        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BossState.Shooting:
                UpdateShooting();
                break;

            case BossState.DivePause:
                animator.SetBool("DiveAttack", true);
                UpdateDivePause();
                break;

            case BossState.Diving:
                UpdateDiving();
                break;

            case BossState.Returning:
                animator.SetBool("DiveAttack", false);
                UpdateReturning();
                break;
        }
    }

    // ================= STATE MACHINE =================

    void ChangeState(BossState newState)
    {
        currentState = newState;

        Debug.Log("Garuda entered state: " + newState);

        switch (newState)
        {
            case BossState.Shooting:
                stateTimer = shootingDuration;
                break;

            case BossState.DivePause:
                TurnOnTrail();
                stateTimer = divePauseDuration;
                break;

            case BossState.Diving:
                stateTimer = maxDiveDuration;

                diveStartPos = transform.position;

                // Dive toward player's X but fixed ground Y
                diveEndPos = new Vector3(player.position.x, groundYLevel, transform.position.z);

                diveLerpTime = 0f;

                if (damageHitbox != null)
                    damageHitbox.SetActive(true);

                break;

            case BossState.Returning:
                stateTimer = maxReturnDuration;

                returnLerpTime = 0f;

                if (damageHitbox != null)
                    damageHitbox.SetActive(false);
                TurnOffTrailSmooth();
                break;
        }
    }

    // ================= SHOOTING =================

    void UpdateShooting()
    {
        HandlePatrol();
        HandleShooting();

        if (stateTimer <= 0f)
            ChangeState(BossState.DivePause);
    }

    void HandlePatrol()
    {
        Vector3 targetPos = new Vector3(targetX, patrolYLevel, transform.position.z);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Mathf.Abs(transform.position.x - targetX) < 0.1f)
        {
            targetX = Mathf.Approximately(targetX, pointAX) ? pointBX : pointAX;
        }
    }

    void HandleShooting()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            animator.SetTrigger("TornadoShoot");
            shootTimer = 0f;
            Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        }
    }

    // ================= DIVE PAUSE =================

    void UpdateDivePause()
    {
        // Boss stands still during pause
        if (stateTimer <= 0f)
            ChangeState(BossState.Diving);
    }

    // ================= DIVING =================

    void UpdateDiving()
    {
        diveLerpTime += Time.deltaTime * diveSpeed;

        transform.position = Vector3.Lerp(
            diveStartPos,
            diveEndPos,
            diveLerpTime
        );

        // If reached ground OR timer expired
        if (diveLerpTime >= 1f || stateTimer <= 0f)
        {
            ChangeState(BossState.Returning);
        }
    }

    // ================= RETURNING =================

    void UpdateReturning()
    {
        returnLerpTime += Time.deltaTime * returnSpeed;

        transform.position = Vector3.Lerp(
            diveEndPos,
            diveStartPos,
            returnLerpTime
        );

        if (returnLerpTime >= 1f || stateTimer <= 0f)
        {
            ChangeState(BossState.Shooting);
        }
    }

    // ================= FLIP ===================

    void FlipTowardsPlayer()
    {
        float dir = player.position.x - transform.position.x;

        if (dir > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (dir < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void TurnOffTrailSmooth()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }
    }

    public void TurnOnTrail()
    {
        if (trailRenderer != null)
        {

            trailRenderer.emitting = true;
        }
    }
}
