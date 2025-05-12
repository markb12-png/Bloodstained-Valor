using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Timing (30FPS)")]
    [SerializeField] private int startupFrames = 4;
    [SerializeField] private int dashFrames = 6;
    [SerializeField] private int slowdownFrames = 8;
    [SerializeField] private int recoveryFrames = 7;
    [SerializeField] private int hitboxSpawnFrame = 10;
    [SerializeField] private int hitboxDurationFrames = 8;

    [Header("Movement")]
    [SerializeField] private float maxDashSpeed = 10f;
    [SerializeField]
    private AnimationCurve accelerationCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.2f, 1f),
        new Keyframe(1, 0.2f)
    );

    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1f, 0);

    private Rigidbody2D rb;
    private GroundDetector groundDetector;
    private MonoBehaviour[] otherScripts;
    private bool isAttacking;
    private bool interruptedByClash = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
        otherScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != null && script != this && script != groundDetector)
            .ToArray();
    }

    private void Update()
    {
        if (!isAttacking && groundDetector.IsGrounded && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(AttackMovement());

        }
    }

    public void InterruptBySwordClash()
    {
        interruptedByClash = true;
        StopAllCoroutines();
        ToggleOtherScripts(false);
        isAttacking = false;
    }

    private IEnumerator AttackMovement()
    {
        isAttacking = true;
        interruptedByClash = false;
        ToggleOtherScripts(false);

        Vector2 dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        int currentFrame = 0;

        rb.velocity = Vector2.zero;

        while (currentFrame < startupFrames)
        {
            if (interruptedByClash) yield break;
            currentFrame++;
            yield return null;
        }

        while (currentFrame < startupFrames + dashFrames)
        {
            if (interruptedByClash) yield break;

            float progress = (currentFrame - startupFrames) / (float)dashFrames;
            float speed = maxDashSpeed * accelerationCurve.Evaluate(progress);
            rb.velocity = dashDirection * speed;

            if (currentFrame == hitboxSpawnFrame)
            {
                SpawnHitbox(dashDirection);
            }

            currentFrame++;
            yield return null;
        }

        float initialSlowdownSpeed = rb.velocity.magnitude;
        while (currentFrame < startupFrames + dashFrames + slowdownFrames)
        {
            if (interruptedByClash) yield break;

            float progress = (currentFrame - startupFrames - dashFrames) / (float)slowdownFrames;
            float speed = Mathf.Lerp(initialSlowdownSpeed, 0, progress * progress);
            rb.velocity = dashDirection * speed;
            currentFrame++;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        while (currentFrame < startupFrames + dashFrames + slowdownFrames + recoveryFrames)
        {
            if (interruptedByClash) yield break;
            currentFrame++;
            yield return null;
        }

        ToggleOtherScripts(true);
        isAttacking = false;
    }

    private void SpawnHitbox(Vector2 direction)
    {

        var shake = Camera.main?.GetComponent<SlightCameraShake>();
        if (shake != null)
        {
            shake.Shake();
        }

        if (hitboxPrefab == null) return;

        Vector2 spawnPosition = (Vector2)transform.position + hitboxOffset * direction;
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPosition, Quaternion.identity, transform); // parented
        HitboxDamage damage = hitbox.GetComponent<HitboxDamage>();
        if (damage != null)
        {
            damage.SetOwner(gameObject);
        }

        Destroy(hitbox, hitboxDurationFrames / 30f);
    }

    private void ToggleOtherScripts(bool state)
    {
        foreach (var script in otherScripts)
        {
            if (script != null) script.enabled = state;
        }
    }
}
