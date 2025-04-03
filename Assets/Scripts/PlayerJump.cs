using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 10f; // Adjustable jump force
    public int startupFrames = 4; // Frames to wait before applying jump force
    public int lockFramesAfterLanding = 5; // Frames after landing to keep locked
    private bool isJumping = false; // Tracks if the player is in a jump sequence

    [Header("References")]
    private Rigidbody2D rb; // Reference to the Rigidbody2D
    private GroundCheck groundCheck; // Reference to the GroundCheck script
    private List<MonoBehaviour> allScripts; // List to store other scripts on this GameObject

    void Start()
    {
        // Cache components
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();

        // Populate allScripts list
        allScripts = new List<MonoBehaviour>(GetComponents<MonoBehaviour>());
        allScripts.Remove(this); // Exclude PlayerJump script itself

        // Remove null or missing scripts to prevent issues
        allScripts.RemoveAll(script => script == null);

        // Debug: Log all added scripts for verification
        foreach (var script in allScripts)
        {
            Debug.Log("Script added to allScripts: " + script.GetType().Name);
        }
    }

    void Update()
    {
        // Prevent jump input during a jump sequence
        if (isJumping) return;

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space) && groundCheck.IsGrounded())
        {
            StartCoroutine(JumpSequence());
        }
    }

    private IEnumerator JumpSequence()
    {
        isJumping = true; // Lock further actions

        // Disable all other scripts
        DisableAllScripts();

        // Step 1: Startup delay
        for (int i = 0; i < startupFrames; i++)
        {
            yield return null; // Wait for one frame
        }

        // Step 2: Apply upward jump force while retaining horizontal velocity
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // Step 3: Wait until the player lands
        yield return new WaitUntil(() => groundCheck.IsGrounded());

        // Step 4: Lock actions for extra frames after landing
        for (int i = 0; i < lockFramesAfterLanding; i++)
        {
            yield return null;
        }

        // Step 5: Re-enable all scripts and unlock actions
        EnableAllScripts();
        isJumping = false;
    }

    private void DisableAllScripts()
    {
        foreach (var script in allScripts)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }

    private void EnableAllScripts()
    {
        foreach (var script in allScripts)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }
    }
}
