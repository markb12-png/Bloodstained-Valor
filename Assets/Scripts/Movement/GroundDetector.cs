using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f); // Center of character

    [Header("Debug")]
    public bool showDebugRays = true;
    public Color groundHitColor = Color.green;
    public Color noGroundColor = Color.red;

    public bool IsGrounded { get; private set; }

    void Update()
    {
        CheckGround();
    }

    void CheckGround()
    {
        Vector2 rayOrigin = (Vector2)transform.position + groundCheckOffset;

        // Single raycast version
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        IsGrounded = hit.collider != null;

        // Debug visualization
        if (showDebugRays)
        {
            Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance,
                         IsGrounded ? groundHitColor : noGroundColor);
        }
    }
}