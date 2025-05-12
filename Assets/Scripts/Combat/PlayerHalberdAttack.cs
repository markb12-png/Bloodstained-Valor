using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerHalberdAttack : MonoBehaviour
{
    [Header("Timing (30FPS)")]
    [SerializeField] private int startupFrames = 20;
    [SerializeField] private int dashFrames = 4;
    [SerializeField] private int slowdownFrames = 8;
    [SerializeField] private int recoveryFrames = 20;
    [SerializeField] private int hitboxSpawnFrame = 24;
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
        if (!isAttacking && groundDetector.IsGrounded && Input.GetMouseButtonDown(1))
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
        int totalFrames = startupFrames + dashFrames + slowdownFrames + recoveryFrames;

        rb.velocity = Vector2.zero;

        while (currentFrame < totalFrames)
        {
            if (interruptedByClash) yield break;

            if (currentFrame == hitboxSpawnFrame)
            {
                SpawnHitbox(dashDirection);
            }

            if (currentFrame < startupFrames)
            {
                rb.velocity = Vector2.zero;
            }
            else if (currentFrame < startupFrames + dashFrames)
            {
                float progress = (currentFrame - startupFrames) / (float)dashFrames;
                float speed = maxDashSpeed * accelerationCurve.Evaluate(progress);
                rb.velocity = dashDirection * speed;
            }
            else if (currentFrame < startupFrames + dashFrames + slowdownFrames)
            {
                float progress = (currentFrame - startupFrames - dashFrames) / (float)slowdownFrames;
                float speed = Mathf.Lerp(maxDashSpeed, 0, progress * progress);
                rb.velocity = dashDirection * speed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

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
