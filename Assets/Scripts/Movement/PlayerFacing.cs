using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    public Rigidbody2D rb;
    private float lastDirection = 1f; // 1 = right, -1 = left

    void Update()
    {
        float vx = rb.velocity.x;

        // Update facing only when moving horizontally
        if (vx > 0.01f)
        {
            lastDirection = 1f;
            FaceRight();
        }
        else if (vx < -0.01f)
        {
            lastDirection = -1f;
            FaceLeft();
        }
    }

    private void FaceRight()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void FaceLeft()
    {
        Vector3 scale = transform.localScale;
        scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public int GetFacingDirection()
    {
        return (int)Mathf.Sign(lastDirection); // Returns -1 (left) or 1 (right)
    }
}
