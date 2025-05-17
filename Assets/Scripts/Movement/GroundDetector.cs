using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f);

    public bool IsGrounded;

    void Update()
    {
        Vector2 rayOrigin = (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        IsGrounded = hit.collider != null;
    }
}
