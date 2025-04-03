using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public LayerMask groundLayer; // LayerMask to specify ground objects
    public float groundCheckDistance = 0.2f; // Distance for the raycast
    public Vector2 raycastOffset = new Vector2(0f, -0.5f); // Offset for the raycast origin

    public bool IsGrounded()
    {
        // Calculate the raycast origin (shifted downward)
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y) + raycastOffset;

        // Perform a single raycast downward from the adjusted position
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);

        // Visualize the raycast for debugging (ensure only one line is drawn)
        Debug.DrawLine(rayOrigin, rayOrigin + Vector2.down * groundCheckDistance, Color.red);

        // Return true if the raycast hits something, false otherwise
        return hit.collider != null;
    }


}
