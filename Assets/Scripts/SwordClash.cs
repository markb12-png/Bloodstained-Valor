using System.Collections;
using UnityEngine;

public class SwordClash : MonoBehaviour
{
    [Header("Settings")]
    public float pushBackForce = 10f; // Force applied to push the player back
    public int freezeFrames = 4; // Number of frames to freeze the game
    public int lockFrames = 30; // Number of frames to lock actions after the clash

    [Header("References")]
    private Rigidbody2D rb; // Reference to the Rigidbody2D of the player
    private PlayerAttack playerAttackScript; // Reference to the PlayerAttack script

    void Start()
    {
        // Initialize Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Missing Rigidbody2D component on " + gameObject.name);
        }

        // Find and reference the PlayerAttack script
        playerAttackScript = GetComponent<PlayerAttack>();
        if (playerAttackScript == null)
        {
            Debug.LogWarning("No PlayerAttack script found on " + gameObject.name + ". SwordClash cannot override it.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Detect collision with another hitbox
        Debug.Log("OnTriggerEnter2D triggered. Checking for hitbox...");
        if (other.CompareTag("Hitbox") && gameObject.CompareTag("Hitbox") && !PriorityManager.isClashActive)
        {
            Debug.Log("Hitbox collision detected! Starting clash sequence.");
            StartCoroutine(ClashSequence(other));
        }
        else
        {
            Debug.Log("Collision detected but it was not a valid hitbox interaction or a clash is already active.");
        }
    }

    private IEnumerator ClashSequence(Collider2D other)
    {
        Debug.Log("ClashSequence started!");

        // Cancel PlayerAttack logic
        if (playerAttackScript != null && playerAttackScript.enabled)
        {
            Debug.Log("Cancelling PlayerAttack and overriding with SwordClash logic.");
            StopAllCoroutines(); // Stop all coroutines on PlayerAttack
            playerAttackScript.enabled = false; // Disable PlayerAttack script
        }

        // Set clash priority
        PriorityManager.isClashActive = true;
        PriorityManager.OverrideWithClash(gameObject);

        // Step 1: Freeze the game for the specified number of frames
        Debug.Log("Freezing game for " + freezeFrames + " frames.");
        Time.timeScale = 0.01f;
        for (int i = 0; i < freezeFrames; i++)
        {
            yield return new WaitForSecondsRealtime(0.01f); // Wait for real time
        }
        Time.timeScale = 1f;
        Debug.Log("Game freeze completed.");

        // Step 2: Check if the 'other' collider still exists
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("The other collider was destroyed during the clash.");
            PriorityManager.RestorePriority(gameObject);
            PriorityManager.isClashActive = false;
            yield break;
        }

        // Step 3: Apply push-back force
        Vector2 clashDirection = (transform.position - other.transform.position).normalized;
        rb.AddForce(clashDirection * pushBackForce, ForceMode2D.Impulse);
        Debug.DrawRay(transform.position, clashDirection * 2, Color.red, 1f);
        Debug.Log("Push-back force applied. Direction: " + clashDirection + ", Magnitude: " + pushBackForce);

        // Step 4: Lock player actions for the specified number of frames
        Debug.Log("Locking player actions for " + lockFrames + " frames.");
        for (int i = 0; i < lockFrames; i++)
        {
            yield return null; // Wait for one frame
        }

        // Step 5: Restore scripts and priority state
        Debug.Log("Restoring lower-priority scripts.");
        PriorityManager.RestorePriority(gameObject);
        PriorityManager.isClashActive = false;

        // Re-enable PlayerAttack after clash ends
        if (playerAttackScript != null)
        {
            playerAttackScript.enabled = true;
            Debug.Log("PlayerAttack script re-enabled.");
        }

        Debug.Log("ClashSequence finished!");
    }
}
