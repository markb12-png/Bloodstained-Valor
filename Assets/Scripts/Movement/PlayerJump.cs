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
        if (isJumping)
            Player.state = Player.Move.isJumping;

        // Enable/disable air attack
        if (airAttack != null)
        {
            airAttack.enabled = !IsGrounded();
        }

        // Air attack override using Input Manager
        if (isJumping && airAttack != null && !IsGrounded() && Input.GetButtonDown("AirAtt"))
        {
            Debug.Log("[PlayerJump] Interrupted by Air Attack");
            if (currentJumpRoutine != null)
                StopCoroutine(currentJumpRoutine);

            ToggleOtherScripts(false);
            airAttack.enabled = true;
            airAttack.TriggerAirAttack(OnAirAttackFinished);
            return;
        }

        if (isJumping) return;

        // Jump using Input Manager
        if (Input.GetButtonDown("Jump") && IsGrounded())
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
        Debug.Log("JUMP SEQUENCE START");

        if (animator != null)
        {
            animator.Play("jump start");
            jumpStartPlaying = true;
            StartCoroutine(WatchJumpStartAnimation());
        }

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        yield return new WaitUntil(() => !IsGrounded());
        Debug.Log("Player in air");
        yield return new WaitUntil(() => IsGrounded());
        Debug.Log("Player landed");

        // Play landing animation
        animator.Play("jump land");

        Debug.Log("WAITING JUMP LAND ANIMATION END");
        while (!IsAnimationFinished("jump land"))
        {
            yield return null;
        }
        Debug.Log("Jump land animation ended");

        // Lock player for a short period (landing lock)
        for (int i = 0; i < lockFramesAfterLanding; i++)
        {
            yield return null;
        }

        // After lock: Play the correct movement/idle animation based on input & velocity
        float moveInput = Input.GetAxisRaw("Horizontal");
        float velocityX = rb.velocity.x;
        float minWalkSpeed = 0.1f;

        if (Mathf.Abs(moveInput) > 0.01f && Mathf.Abs(velocityX) > minWalkSpeed)
        {
            // Walking/running
            if (velocityX < 0)
                animator.Play("walk left");   // Replace with your run/walk left animation if needed
            else
                animator.Play("walk right");  // Replace with your run/walk right animation if needed
        }
        else
        {
            // Idle
            if (velocityX < 0)
                animator.Play("idle animation left");
            else
                animator.Play("idle animation right");
        }

        Debug.Log("Resume scripts");
        ToggleOtherScripts(true);
        isJumping = false;
    }



    private IEnumerator WatchJumpStartAnimation()
    {
        Debug.Log("WATCHING JUMP START ANIMATION END");
        while (!IsAnimationFinished("jump start"))
        {
            yield return null;
        }
        Debug.Log("jumpStartPlaying = false");
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
