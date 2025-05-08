using UnityEngine;

public class SlightCameraShake : MonoBehaviour
{
    private float shakeDuration = 0.2f;      // Default duration of shake
    private float shakeMagnitude = 0.1f;     // Default magnitude of shake
    private float currentShakeTime = 0f;
    private Vector3 shakeOffset = Vector3.zero;
    private bool isShaking = false;

    void Update()
    {
        // Test trigger
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartShake();
        }

        if (isShaking)
        {
            // Reduce shake time
            currentShakeTime -= Time.deltaTime;

            // Remove previous shake offset
            transform.localPosition -= shakeOffset;

            if (currentShakeTime > 0f)
            {
                Vector2 offset2D = Random.insideUnitCircle * shakeMagnitude;
                shakeOffset = new Vector3(offset2D.x, offset2D.y, 0f);
                transform.localPosition += shakeOffset;
            }
            else
            {
                shakeOffset = Vector3.zero;
                isShaking = false;
                Debug.Log("[SlightCameraShake] Shake ended.");
            }
        }
    }

    public void StartShake()
    {
        if (!isShaking)
        {
            isShaking = true;
            currentShakeTime = shakeDuration;
            Debug.Log("[SlightCameraShake] Shake started.");
        }
    }
    public void Shake()
    {
        StartShake();
    }

}
