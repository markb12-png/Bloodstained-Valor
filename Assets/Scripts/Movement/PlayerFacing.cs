using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    private Rigidbody2D rb; // Reference to the Rigidbody2D

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Continuously check and enforce the facing direction based solely on horizontal velocity
        if (rb.velocity.x > 0.01f)
        {
            ForceFaceRight();
        }
        else if (rb.velocity.x < -0.01f)
        {
            ForceFaceLeft();
        }
    }

    private void ForceFaceRight()
    {
        // Directly enforce facing right by setting the scale and ensuring no interference
        Vector3 scale = transform.localScale;
        if (scale.x < 0) // Only flip if facing left
        {
            scale.x = Mathf.Abs(scale.x); // Set X scale to positive
            transform.localScale = scale;
        }
    }

    private void ForceFaceLeft()
    {
        // Directly enforce facing left by setting the scale and ensuring no interference
        Vector3 scale = transform.localScale;
        if (scale.x > 0) // Only flip if facing right
        {
            scale.x = -Mathf.Abs(scale.x); // Set X scale to negative
            transform.localScale = scale;
        }
    }

    private void LateUpdate()
    {
        // Enforce facing direction in LateUpdate to overwrite any changes by other scripts
        Update(); // Call the Update logic again at the end of the frame
    }
}
