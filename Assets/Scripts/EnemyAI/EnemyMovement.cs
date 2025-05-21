using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float backdashDistance = 1f;
    [SerializeField] private float backdashDuration = 0.6f;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isBackdashing = false;
    private int originalLayer;

    [Header("Debug")]
    [SerializeField] private bool enableManualBackdash = false; // Enable to test with K key
    [SerializeField] private Transform playerTransform; // Needed for direction when testing

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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

        float dir = Mathf.Sign(directionToPlayer.x);
        transform.position += new Vector3(speed * dir, 0, 0);
        animator.Play("enemy forwards walk");
    }

    public void WalkAway(Vector2 directionToPlayer)
    {
        if (isBackdashing) return;

        float dir = Mathf.Sign(directionToPlayer.x);
        transform.position -= new Vector3(speed * dir, 0, 0);
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
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");

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

        // Return to idle or walk animation based on velocity
        if (rb.velocity.magnitude < 0.01f)
        {
            animator.Play("enemy idle");
        }
        else if (rb.velocity.x > 0)
        {
            animator.Play("enemy forwards walk");
        }
        else
        {
            animator.Play("enemy backwards walk");
        }
    }
}
