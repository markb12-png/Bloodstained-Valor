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

    private Rigidbody2D rb;
    private GroundDetector groundDetector;
    private PlayerJump jumpScript;
    private PlayerFacing facingScript;
    private AirSwordClashHandler clashHandler;
    private MonoBehaviour[] otherScripts;
    private bool isAirAttacking = false;
    private bool interruptedByClash = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
        jumpScript = GetComponent<PlayerJump>();
        facingScript = GetComponent<PlayerFacing>();
        clashHandler = GetComponent<AirSwordClashHandler>();

        otherScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != null && script != this && !(script is GroundDetector))
            .ToArray();
    }

    public void TriggerAirAttack(Action onComplete)
    {
        if (!isAirAttacking)
        {
            StartCoroutine(AirAttackSequence(onComplete));
        }
    }

    public void InterruptBySwordClash()
    {
        Debug.Log("[PlayerAirAttack] Interrupted by sword clash.");
        interruptedByClash = true;
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        ToggleOtherScripts(false);
        isAirAttacking = false;
    }

    private IEnumerator AirAttackSequence(Action onComplete)
    {
        isAirAttacking = true;
        interruptedByClash = false;
        ToggleOtherScripts(false);
        jumpScript.enabled = false;

        int direction = facingScript != null ? facingScript.GetFacingDirection() : 1;
        int currentFrame = 0;
        bool hitboxSpawned = false;

        rb.velocity = Vector2.zero;
        while (currentFrame < windupFrames)
        {
            if (interruptedByClash) yield break;
            rb.velocity = new Vector2(direction * 2f, upwardVelocity);
            currentFrame++;
            yield return null;
        }

        currentFrame = 0;
        rb.velocity = new Vector2(direction * 2f, downwardVelocity);

        GameObject hitboxInstance = null;

        while (currentFrame < dashFrames)
        {
            if (interruptedByClash) yield break;

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
            if (interruptedByClash) yield break;
            yield return null;
        }

        EnableAllScripts();
        isAirAttacking = false;
        onComplete?.Invoke();
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
            if (script != null)
                script.enabled = state;
        }
    }

    private void EnableAllScripts()
    {
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != null) script.enabled = true;
        }
    }

    public bool IsAirAttacking()
    {
        return isAirAttacking;
    }
}
