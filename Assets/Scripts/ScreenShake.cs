using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Header("Settings")]
    public float maxShakeMagnitude = 0.5f; // Maximum intensity of the shake
    public int shakeDecayFrames = 14; // Number of frames for the shake effect to decay

    private Camera mainCamera; // Reference to the camera for shaking

    void Start()
    {
        // Get the camera reference
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found. Screen shake effect will not work.");
        }
        else
        {
            Debug.Log("Main camera successfully found.");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collision detected with {collision.gameObject.name}");

        // Check if both objects are hitboxes
        if (collision.CompareTag("Hitbox") && gameObject.CompareTag("Hitbox"))
        {
            Debug.Log("Collision detected between hitboxes! Triggering screen shake.");
            StartCoroutine(IntenseScreenShake(shakeDecayFrames, maxShakeMagnitude));
        }
    }

    private IEnumerator IntenseScreenShake(int frames, float initialMagnitude)
    {
        if (mainCamera == null) yield break;

        Vector3 originalPosition = mainCamera.transform.position;

        for (int frame = 0; frame < frames; frame++)
        {
            // Calculate the intensity based on the decay
            float currentMagnitude = Mathf.Lerp(initialMagnitude, 0, frame / (float)frames);

            // Apply random shake to the camera's position
            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;

            mainCamera.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            Debug.Log($"Screen shake frame {frame + 1}/{frames}: Magnitude = {currentMagnitude}");

            yield return null; // Wait for the next frame
        }

        // Restore the camera to its original position
        mainCamera.transform.position = originalPosition;
        Debug.Log("Screen shake effect completed.");
    }
}
