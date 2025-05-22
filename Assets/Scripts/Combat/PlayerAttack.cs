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

    [Header("Cooldown")]
    [SerializeField] private float cooldownTime = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private GroundDetector groundDetector;
    private MonoBehaviour[] otherScripts;

    private bool isAttacking = false;
    private Coroutine attackCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundDetector = GetComponent<GroundDetector>();

        otherScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != null && script != this && script != groundDetector)
            .ToArray();

        Player.canAttack = true;
        Player.isAttacking = false;
    }

    private void Update()
    {
        bool inputPressed = Input.GetButtonDown("Attack");
        bool grounded = groundDetector != null && groundDetector.IsGrounded;
        bool canAttackNow = !isAttacking && Player.canAttack;

        if (inputPressed && grounded && canAttackNow)
        {
            attackCoroutine = StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        Player.isAttacking = true;
        Player.canAttack = false;

        ToggleOtherScripts(false);

        Vector2 dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        animator?.Play("knight sword attack");

        rb.velocity = Vector2.zero;
        int currentFrame = 0;

        while (currentFrame < startupFrames)
        {
            currentFrame++;
            yield return null;
        }

        while (currentFrame < startupFrames + dashFrames)
        {
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

        float initialSpeed = rb.velocity.magnitude;
        while (currentFrame < startupFrames + dashFrames + slowdownFrames)
        {
            float progress = (currentFrame - startupFrames - dashFrames) / (float)slowdownFrames;
            float speed = Mathf.Lerp(initialSpeed, 0, progress * progress);
            rb.velocity = dashDirection * speed;

            currentFrame++;
            yield return null;
        }

        rb.velocity = Vector2.zero;

        while (currentFrame < startupFrames + dashFrames + slowdownFrames + recoveryFrames)
        {
            currentFrame++;
            yield return null;
        }

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName("knight sword attack") && state.normalizedTime >= 1f;
        });

        animator.Play(rb.velocity.x < 0 ? "idle animation left" : "idle animation right");

        ToggleOtherScripts(true);
        isAttacking = false;
        Player.isAttacking = false;

        yield return new WaitForSeconds(cooldownTime);
        Player.canAttack = true;
    }

    public void ForceCancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        ToggleOtherScripts(true);
        isAttacking = false;
        Player.isAttacking = false;
        Player.canAttack = true;

        if (animator != null)
        {
            animator.Play(rb.velocity.x < 0 ? "idle animation left" : "idle animation right");
        }
    }

    private void SpawnHitbox(Vector2 direction)
    {
        if (hitboxPrefab == null)
        {
            Debug.LogWarning("[Attack] No hitboxPrefab assigned!");
            return;
        }

        Vector2 spawnPosition = (Vector2)transform.position + hitboxOffset * direction;
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPosition, Quaternion.identity, transform);
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
            if (script == null || script is PlayerHealth)
                continue;

            script.enabled = state;
        }
    }

    public bool IsPlayerAttacking()
    {
        return isAttacking;
    }
}