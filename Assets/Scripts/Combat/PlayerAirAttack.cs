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
    [SerializeField] private float upwardLift = 4f;
    [SerializeField] private float downwardSpeed = -20f;

    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Vector2 hitboxOffset = new Vector2(0f, -1f);

    private Rigidbody2D rb;
    private GroundDetector groundDetector;
    private MonoBehaviour[] otherScripts;
    private PlayerJump jumpScript;
    private bool isAirAttacking;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
        jumpScript = GetComponent<PlayerJump>();

        otherScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this && script != groundDetector)
            .ToArray();
    }

    void Update()
    {
        // Only allow air attack when not grounded
        if (!groundDetector.IsGrounded && !isAirAttacking && Input.GetMouseButtonDown(0))
        {
            Debug.Log("[AirAttack] Triggering air attack");
            StartCoroutine(AirAttackSequence());
        }
    }

    private IEnumerator AirAttackSequence()
    {
        isAirAttacking = true;
        ToggleOtherScripts(false);
        jumpScript.enabled = false;

        int currentFrame = 0;

        // Windup phase (player floats up)
        rb.velocity = new Vector2(0, upwardLift);
        while (currentFrame < windupFrames)
        {
            currentFrame++;
            yield return null;
        }

        // Dash downward
        rb.velocity = new Vector2(0, downwardSpeed);
        while (currentFrame < windupFrames + dashFrames)
        {
            // Spawn hitbox once at correct frame
            if (currentFrame == hitboxSpawnFrame)
            {
                Debug.Log("[AirAttack] Spawning vertical hitbox");
                SpawnHitbox();
            }

            currentFrame++;
            yield return null;
        }

        // Recovery phase (stall in place)
        rb.velocity = Vector2.zero;
        while (currentFrame < windupFrames + dashFrames + recoveryFrames)
        {
            currentFrame++;
            yield return null;
        }

        ToggleOtherScripts(true);
        jumpScript.enabled = true;
        isAirAttacking = false;
    }

    private void SpawnHitbox()
    {
        if (hitboxPrefab == null) return;

        Vector2 spawnPosition = (Vector2)transform.position + hitboxOffset;
        Quaternion rotation = Quaternion.Euler(0, 0, -90); // Face downward

        GameObject hitbox = Instantiate(hitboxPrefab, spawnPosition, rotation);
        Destroy(hitbox, hitboxDurationFrames / 30f);

        Debug.Log($"[AirAttack] Hitbox spawned at {spawnPosition}");
    }

    private void ToggleOtherScripts(bool state)
    {
        foreach (var script in otherScripts)
        {
            if (script != null)
                script.enabled = state;
        }
    }
}
