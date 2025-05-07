using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerHalberdAttack : MonoBehaviour
{
    [Header("Timing (30FPS)")]
    [SerializeField] private int windupFrames = 20;
    [SerializeField] private int dashFrames = 4;
    [SerializeField] private int recoveryFrames = 28;
    [SerializeField] private int hitboxSpawnFrame = 24;
    [SerializeField] private int hitboxDurationFrames = 8;

    [Header("Movement")]
    [SerializeField] private float dashDistance = 3f;
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
    private MonoBehaviour[] otherScripts;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        otherScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this)
            .ToArray();
    }

    void Update()
    {
        if (!isAttacking && Input.GetMouseButtonDown(1)) // RMB
        {
            StartCoroutine(HalberdAttack());
        }
    }

    private IEnumerator HalberdAttack()
    {
        isAttacking = true;
        ToggleOtherScripts(false);

        rb.velocity = Vector2.zero;
        int currentFrame = 0;
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 startPosition = rb.position;
        Vector2 endPosition = startPosition + Vector2.right * direction * dashDistance;

        // Windup
        while (currentFrame < windupFrames)
        {
            currentFrame++;
            yield return null;
        }

        // Dash with interpolation
        for (int i = 0; i < dashFrames; i++)
        {
            float t = (float)i / (dashFrames - 1);
            float easedT = accelerationCurve.Evaluate(t);
            Vector2 dashPos = Vector2.Lerp(startPosition, endPosition, easedT);
            rb.MovePosition(dashPos);

            if (currentFrame == hitboxSpawnFrame)
            {
                SpawnHitbox(direction);
            }

            currentFrame++;
            yield return null;
        }

        // Recovery
        rb.velocity = Vector2.zero;
        while (currentFrame < windupFrames + dashFrames + recoveryFrames)
        {
            currentFrame++;
            yield return null;
        }

        ToggleOtherScripts(true);
        isAttacking = false;
    }

    private void SpawnHitbox(float direction)
    {
        if (hitboxPrefab == null) return;

        Vector2 spawnPosition = (Vector2)transform.position + new Vector2(hitboxOffset.x * direction, hitboxOffset.y);
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPosition, Quaternion.identity);
        Destroy(hitbox, hitboxDurationFrames / 30f);

        Debug.Log("[PlayerHalberdAttack] Hitbox spawned");
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
