using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 0.2f;
    [SerializeField] private bool useUnscaledTime = false;

    private float currentShakeTime = 0f;
    private Vector3 shakeOffset = Vector3.zero;
    private bool isShaking = false;

    void Update()
    {
        // Test trigger
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartShake();
        }

        if (isShaking)
        {
            float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            currentShakeTime -= deltaTime;

            // Remove previous frame's shake offset
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
                Debug.Log("[CameraShake] Shake ended.");
            }
        }
    }

    public void StartShake()
    {
        if (!isShaking)
        {
            isShaking = true;
            currentShakeTime = shakeDuration;
            Debug.Log("[CameraShake] Shake started.");
        }
    }

    public void Shake()
    {
        StartShake();
    }

}
