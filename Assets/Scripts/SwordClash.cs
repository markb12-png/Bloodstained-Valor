using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordClash : MonoBehaviour
{
    [Header("Settings")]
    public float pushBackForce = 10f; // Force applied to push the capsule player back
    public int freezeFrames = 4; // Number of frames to pause the game
    public int immobileFrames = 26; // Number of frames the player remains immobile

    [Header("References")]
    private Rigidbody2D rb; // Reference to the player's Rigidbody2D
    private List<MonoBehaviour> otherScripts; // Stores all other scripts to disable
    private bool isClashing = false; // Tracks whether a clash sequence is ongoing

    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing on " + gameObject.name);
        }

        // Collect all other scripts on this GameObject
        otherScripts = new List<MonoBehaviour>(GetComponents<MonoBehaviour>());
        otherScripts.Remove(this); // Exclude the SwordClash script itself
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent multiple clashes from overlapping
        if (isClashing) return;

        // Check if the collision is between two hitboxes
        if (other.CompareTag("Hitbox") && gameObject.CompareTag("Hitbox"))
        {
            Debug.Log("Hitbox collision detected! Starting clash sequence.");
            StartCoroutine(ClashSequence(other));
        }
    }

    private IEnumerator ClashSequence(Collider2D other)
    {
        isClashing = true;

        // Disable all other scripts to prioritize SwordClash
        DisableOtherScripts();

        // Step 1: Pause the game for the specified number of frames
        Debug.Log("Pausing game for " + freezeFrames + " frames.");
        Time.timeScale = 0f; // Pause the game
        for (int i = 0; i < freezeFrames; i++)
        {
            yield return null; // Wait for one frame
        }
        Time.timeScale = 1f; // Resume the game
        Debug.Log("Game resumed.");

        // Step 2: Apply push-back force to the player
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("Other collider was destroyed during the clash.");
            RestoreOtherScripts();
            isClashing = false;
            yield break;
        }

        Vector2 clashDirection = (transform.position - other.transform.position).normalized;
        rb.velocity = Vector2.zero; // Reset velocity before applying force
        rb.AddForce(clashDirection * pushBackForce, ForceMode2D.Impulse);
        Debug.Log("Push-back force applied. Direction: " + clashDirection + ", Magnitude: " + pushBackForce);

        // Step 3: Lock the player for the specified number of immobile frames
        Debug.Log("Player immobile for " + immobileFrames + " frames.");
        for (int i = 0; i < immobileFrames; i++)
        {
            yield return null; // Wait for one frame
        }
        Debug.Log("Player is now free to act.");

        // Restore all other scripts after the clash sequence
        RestoreOtherScripts();

        isClashing = false; // End the clash sequence
        Debug.Log("Clash sequence finished.");
    }

    private void DisableOtherScripts()
    {
        foreach (var script in otherScripts)
        {
            if (script != null)
            {
                script.enabled = false; // Disable the script
                Debug.Log("Disabled script: " + script.GetType().Name);
            }
        }
    }

    private void RestoreOtherScripts()
    {
        foreach (var script in otherScripts)
        {
            if (script != null)
            {
                script.enabled = true; // Re-enable the script
                Debug.Log("Restored script: " + script.GetType().Name);
            }
        }
    }
}
