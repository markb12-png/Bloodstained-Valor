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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
        otherScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this && script != groundDetector)
            .ToArray();
    }

    void Update()
    {
        if (!isAttacking && groundDetector.IsGrounded && Input.GetMouseButtonDown(0))
        {
            Debug.Log("[PlayerAttack] Attack started");
            StartCoroutine(AttackMovement());
        }
    }

    private IEnumerator AttackMovement()
    {
        isAttacking = true;
        ToggleOtherScripts(false);
        Vector2 dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        int currentFrame = 0;

        // Startup Phase
        rb.velocity = Vector2.zero;
        while (currentFrame < startupFrames)
        {
            currentFrame++;
            yield return null;
        }

        // Dash Phase
        while (currentFrame < startupFrames + dashFrames)
        {
            float progress = (currentFrame - startupFrames) / (float)dashFrames;
            float speed = maxDashSpeed * accelerationCurve.Evaluate(progress);
            rb.velocity = dashDirection * speed;

            // Spawn the hitbox when it's time
            if (currentFrame == hitboxSpawnFrame)
            {
                Debug.Log("[PlayerAttack] Spawning hitbox");
                SpawnHitbox(dashDirection);
            }

            currentFrame++;
            yield return null;
        }

        // Slowdown Phase
        float initialSlowdownSpeed = rb.velocity.magnitude;
        while (currentFrame < startupFrames + dashFrames + slowdownFrames)
        {
            float progress = (currentFrame - startupFrames - dashFrames) / (float)slowdownFrames;
            float speed = Mathf.Lerp(initialSlowdownSpeed, 0, progress * progress);
            rb.velocity = dashDirection * speed;
            currentFrame++;
            yield return null;
        }

        // Recovery Phase
        rb.velocity = Vector2.zero;
        while (currentFrame < startupFrames + dashFrames + slowdownFrames + recoveryFrames)
        {
            currentFrame++;
            yield return null;
        }

        ToggleOtherScripts(true);
        isAttacking = false;
    }

    private void SpawnHitbox(Vector2 direction)
    {
        if (hitboxPrefab == null) return;

        Vector2 spawnPosition = (Vector2)transform.position + hitboxOffset * direction;

        // Instantiate the hitbox at the correct position
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPosition, Quaternion.identity);

        // Debugging for when the hitbox is created
        Debug.Log($"[PlayerAttack] Hitbox created at position: {spawnPosition}");

        // Optional: Destroy after fixed time even if no hit occurs
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
