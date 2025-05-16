using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerAirAttack : MonoBehaviour
{
    [Header("Timing (30FPS)")]
    [SerializeField] private int windupFrames = 10;
    [SerializeField] private int dashFrames = 6;
    [SerializeField] private int recoveryFrames = 15;
    [SerializeField] private int hitboxSpawnFrame = 10;
    [SerializeField] private int hitboxDurationFrames = 8;

    [Header("Movement")]
    [SerializeField] private float upwardVelocity = 8f;
    [SerializeField] private float downwardVelocity = -20f;

    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Vector2 hitboxOffset = new Vector2(0f, -1f);

    // Cooldown fields removed

    private Rigidbody2D rb;
    private Animator animator;
    private GroundDetector groundDetector;
    private PlayerJump jumpScript;
    private PlayerFacing facingScript;
    private MonoBehaviour[] otherScripts;

    private bool isAirAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundDetector = GetComponent<GroundDetector>();
        jumpScript = GetComponent<PlayerJump>();
        facingScript = GetComponent<PlayerFacing>();

        otherScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != null && script != this && !(script is GroundDetector))
            .ToArray();
    }

    public void TriggerAirAttack(Action onComplete)
    {
        if (!isAirAttacking) // Only check this now
        {
            StartCoroutine(AirAttackSequence(onComplete));
        }
    }

    private IEnumerator AirAttackSequence(Action onComplete)
    {
        isAirAttacking = true;
        ToggleOtherScripts(false);
        jumpScript.enabled = false;

        int direction = facingScript != null ? facingScript.GetFacingDirection() : 1;
        int currentFrame = 0;
        bool hitboxSpawned = false;

        rb.velocity = Vector2.zero;

        if (animator != null)
        {
            animator.Play("knight air attack");
        }

        while (currentFrame < windupFrames)
        {
            rb.velocity = new Vector2(direction * 2f, upwardVelocity);
            currentFrame++;
            yield return null;
        }

        currentFrame = 0;
        rb.velocity = new Vector2(direction * 2f, downwardVelocity);
        GameObject hitboxInstance = null;

        while (currentFrame < dashFrames)
        {
            if (!hitboxSpawned && windupFrames + currentFrame == hitboxSpawnFrame)
            {
                hitboxInstance = SpawnHitbox(direction);
                hitboxSpawned = true;
            }

            if (hitboxInstance != null)
            {
                hitboxInstance.transform.position = transform.position + new Vector3(hitboxOffset.x * direction, hitboxOffset.y, 0);
            }

            currentFrame++;
            yield return null;
        }

        rb.velocity = Vector2.zero;

        for (int i = 0; i < recoveryFrames; i++)
        {
            yield return null;
        }

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName("knight air attack") && state.normalizedTime >= 1f;
        });

        if (rb.velocity.x < 0)
            animator.Play("idle animation left");
        else
            animator.Play("idle animation right");

        EnableAllScripts();
        isAirAttacking = false;
        onComplete?.Invoke();

        // (No cooldown anymore)
    }

    private GameObject SpawnHitbox(int direction)
    {
        var shake = Camera.main?.GetComponent<SlightCameraShake>();
        if (shake != null)
        {
            shake.Shake();
        }

        if (hitboxPrefab == null) return null;

        Vector3 spawnPosition = transform.position + new Vector3(hitboxOffset.x * direction, hitboxOffset.y, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, -90);

        GameObject hitbox = Instantiate(hitboxPrefab, spawnPosition, rotation, transform);

        HitboxDamage damage = hitbox.GetComponent<HitboxDamage>();
        if (damage != null)
        {
            damage.SetOwner(gameObject);
        }

        Destroy(hitbox, hitboxDurationFrames / 30f);
        Debug.Log("[AirAttack] Hitbox spawned and following at " + spawnPosition);

        return hitbox;
    }

    private void ToggleOtherScripts(bool state)
    {
        foreach (var script in otherScripts)
        {
            // Never disable PlayerHealth!
            if (script == null || script is PlayerHealth)
                continue;
            script.enabled = state;
        }
    }

    private void EnableAllScripts()
    {
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            // Never disable PlayerHealth!
            if (script == null || script is PlayerHealth)
                continue;
            script.enabled = true;
        }
    }

    public bool IsAirAttacking()
    {
        return isAirAttacking;
    }
}
