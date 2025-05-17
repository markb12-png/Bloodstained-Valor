using UnityEngine;

public class CameraShake : MonoBehaviour {

    [Header("Shake Settings")]
    [SerializeField] private float shakeMagnitude = 0.2f;

    private float currentShakeTime = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    private bool isShaking = false;
    private float deltaTime = 0f;

    void Update() {

        deltaTime = Time.deltaTime;

        // Test triggers
        // Regular shake logic
        if (Input.GetKeyDown(KeyCode.R)) {
            StartShake(0.2f, 0.3f);
        }
        // Slight shake logic
        if (Input.GetKeyDown(KeyCode.T)) {
            StartShake(0.1f, 0.2f);
        }

        UpdateShake();
    }

    public void StartShake(float magnitude, float duration) {
        if (!isShaking) {
            shakeMagnitude = magnitude;
            isShaking = true;
            currentShakeTime = duration;
            Debug.Log("[CameraShake] Shake started.");
        }
    }

    private void UpdateShake() {
        if (isShaking) {
            currentShakeTime -= deltaTime;

            // Remove previous frame's shake offset
            transform.localPosition -= shakeOffset;

            if (currentShakeTime > 0f) {
                Vector2 offset2D = Random.insideUnitCircle * shakeMagnitude;
                shakeOffset = new Vector3(offset2D.x, offset2D.y, 0f);
                transform.localPosition += shakeOffset;
            }
            else {
                shakeOffset = Vector3.zero;
                isShaking = false;
                Debug.Log("[CameraShake] Shake ended.");
            }
        }
    }
    
}
