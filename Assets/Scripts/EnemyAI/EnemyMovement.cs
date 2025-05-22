using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float backdashDistance = 1f;
    [SerializeField] private float backdashDuration = 0.6f;

    [Header("Debug / Manual Control")]
    [SerializeField] private bool enableManualBackdash = false;
    [SerializeField] private Transform playerTransform;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isBackdashing = false;
    private int originalLayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (animator == null || rb == null)
        {
            Debug.LogError($"{name}: Missing required components on EnemyMovement.");
        }

        originalLayer = gameObject.layer;
    }

    private void Update()
    {
        if (enableManualBackdash && Input.GetKeyDown(KeyCode.L) && playerTransform != null)
        {
            Vector2 directionFromPlayer = transform.position - playerTransform.position;
            Backdash(directionFromPlayer);
        }
    }

    public void WalkToward(Vector2 directionToPlayer)
    {
        if (isBackdashing) return;

        float moveDir = Mathf.Sign(directionToPlayer.x);
        transform.position += new Vector3(speed * moveDir, 0, 0);
        animator.Play("enemy forwards walk");
    }

    public void WalkAway(Vector2 directionToPlayer)
    {
        if (isBackdashing) return;

        float moveDir = Mathf.Sign(directionToPlayer.x);
        transform.position -= new Vector3(speed * moveDir, 0, 0);
        animator.Play("enemy backwards walk");
    }

    public void SetIdleIfStopped()
    {
        if (!isBackdashing && rb.velocity.magnitude < 0.01f)
        {
            animator.Play("enemy idle");
        }
    }

    public void Backdash(Vector2 directionFromPlayer)
    {
        if (isBackdashing) return;

        StartCoroutine(BackdashRoutine(directionFromPlayer));
    }

    private IEnumerator BackdashRoutine(Vector2 directionFromPlayer)
    {
        isBackdashing = true;

        int invulnerableLayer = LayerMask.NameToLayer("Invulnerable");
        if (invulnerableLayer == -1)
        {
            Debug.LogWarning("Layer 'Invulnerable' not found. Backdash will not apply invincibility.");
        }
        else
        {
            gameObject.layer = invulnerableLayer;
        }

        float dir = Mathf.Sign(directionFromPlayer.x);
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(backdashDistance * dir, 0, 0);

        animator.Play("enemy backdash");

        float elapsed = 0f;
        while (elapsed < backdashDuration)
        {
            float t = elapsed / backdashDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        gameObject.layer = originalLayer;
        isBackdashing = false;

        UpdateIdleOrWalk();
    }

    private void UpdateIdleOrWalk()
    {
        float velX = rb.velocity.x;
        if (rb.velocity.magnitude < 0.01f)
        {
            animator.Play("enemy idle");
        }
        else if (velX > 0)
        {
            animator.Play("enemy forwards walk");
        }
        else
        {
            animator.Play("enemy backwards walk");
        }
    }

    public void MoveToward(float targetX)
    {
        if (isBackdashing) return;

        float direction = targetX - transform.position.x;
        WalkToward(new Vector2(direction, 0));
    }

    public void Stop()
    {
        if (!isBackdashing)
        {
            SetIdleIfStopped();
        }
    }
}
