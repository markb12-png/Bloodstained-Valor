using UnityEngine;

public class RaycastTester : MonoBehaviour
{
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.5f;

    void Update()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 1.2f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);

    }
}
