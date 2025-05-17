using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public int lockFramesAfterLanding = 5;
    private bool isJumping = false;
    private Coroutine currentJumpRoutine;

    [Header("Ground Detection")]
    public float groundCheckDistance = 0.2f;
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f);
    public LayerMask groundLayer;

    [Header("References")]
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerAirAttack airAttack;
    private MonoBehaviour[] otherScripts;

    private bool jumpStartPlaying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        airAttack = GetComponent<PlayerAirAttack>();

        otherScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != null && script != this && script != airAttack && script.enabled)
            .ToArray();
    }

    void Update()
    {

        if (isJumping) Player.state = Player.Move.isJumping;

        // Enable/disable air attack
        if (airAttack != null)
        {
            airAttack.enabled = !IsGrounded();
        }

        // Air attack override
        if (isJumping && airAttack != null && !IsGrounded() && Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("[PlayerJump] Interrupted by Air Attack");
            if (currentJumpRoutine != null)
                StopCoroutine(currentJumpRoutine);

            ToggleOtherScripts(false);
            airAttack.TriggerAirAttack(OnAirAttackFinished);
            return;
        }

        if (isJumping) return;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            currentJumpRoutine = StartCoroutine(JumpSequence());
        }

        // Play fall animation once jump start is done
        if (!IsGrounded() && !jumpStartPlaying && isJumping)
        {
            animator.Play("jump fall");
        }
    }

    private bool IsGrounded()
    {
        Vector2 rayOrigin = (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private IEnumerator JumpSequence()
    {
        isJumping = true;
        ToggleOtherScripts(false);

        if (animator != null)
        {
            animator.Play("jump start");
            jumpStartPlaying = true;
            StartCoroutine(WatchJumpStartAnimation());
        }

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        yield return new WaitUntil(() => !IsGrounded());
        yield return new WaitUntil(() => IsGrounded());

        animator.Play("jump land");

        while (!IsAnimationFinished("jump land"))
        {
            yield return null;
        }

        // Idle transition based on direction (no flip)
        if (rb.velocity.x < 0)
        {
            animator.Play("idle animation left");
        }
        else
        {
            animator.Play("idle animation right");
        }

        for (int i = 0; i < lockFramesAfterLanding; i++)
        {
            yield return null;
        }

        ToggleOtherScripts(true);
        isJumping = false;
    }

    private IEnumerator WatchJumpStartAnimation()
    {
        while (!IsAnimationFinished("jump start"))
        {
            yield return null;
        }
        jumpStartPlaying = false;
    }

    private bool IsAnimationFinished(string animationName)
    {
        if (animator == null) return true;
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(animationName) && info.normalizedTime >= 1f;
    }

    private void ToggleOtherScripts(bool state)
    {
        foreach (var script in otherScripts)
        {
            if (script != null)
                script.enabled = state;
        }
    }

    private void OnAirAttackFinished()
    {
        Debug.Log("[PlayerJump] Resuming normal control after air attack");
        ToggleOtherScripts(true);
        isJumping = false;
    }

    // FIX: Add this to reset the jump state after being hit
    public void ResetJumpState()
    {
        isJumping = false;
        jumpStartPlaying = false;
        if (currentJumpRoutine != null)
        {
            StopCoroutine(currentJumpRoutine);
            currentJumpRoutine = null;
        }
        ToggleOtherScripts(true);
    }
}
