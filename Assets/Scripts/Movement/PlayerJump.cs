using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public int startupFrames = 4;
    public int lockFramesAfterLanding = 5;
    private bool isJumping = false;

    [Header("Ground Detection")]
    public float groundCheckDistance = 0.2f;
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f);
    public LayerMask groundLayer;
    public bool showDebugRays = true;

    [Header("References")]
    private Rigidbody2D rb;
    private PlayerAirAttack airAttack;
    private MonoBehaviour[] otherScripts;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        airAttack = GetComponent<PlayerAirAttack>();
        otherScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this && script.enabled)
            .ToArray();
    }

    void Update()
    {
        Vector2 rayOrigin = (Vector2)transform.position + groundCheckOffset;

        // Visualize ground check
        if (showDebugRays)
        {
            Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance,
                         IsGrounded() ? Color.green : Color.red);
        }

        // Enable/disable air attack based on grounded state
        if (airAttack != null)
        {
            airAttack.enabled = !IsGrounded();
        }

        if (isJumping) return;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            StartCoroutine(JumpSequence());
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

        // Startup frames
        for (int i = 0; i < startupFrames; i++)
        {
            yield return null;
        }

        // Apply jump force
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // Wait until we're airborne
        yield return new WaitUntil(() => !IsGrounded());

        // Wait until we land again
        yield return new WaitUntil(() => IsGrounded());

        // Landing lock frames
        for (int i = 0; i < lockFramesAfterLanding; i++)
        {
            yield return null;
        }

        ToggleOtherScripts(true);
        isJumping = false;
    }

    private void ToggleOtherScripts(bool state)
    {
        foreach (var script in otherScripts)
        {
            if (script != null)
            {
                script.enabled = state;
            }
        }
    }
}