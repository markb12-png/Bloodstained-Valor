using UnityEngine;
using System.Collections;

public class CameraShakeRelay : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float defaultMagnitude = 0.15f;
    [SerializeField] private float defaultDuration = 0.3f;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    public void TriggerDefaultShake()
    {
        StartShake(defaultMagnitude, defaultDuration);
    }

    public void TriggerLightShake()
    {
        StartShake(0.07f, 0.15f);
    }

    public void TriggerHeavyShake()
    {
        StartShake(0.3f, 0.4f);
    }

    public void StartShake(float magnitude, float duration)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(magnitude, duration));
    }

    private IEnumerator ShakeRoutine(float magnitude, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = cameraTransform.localPosition;

        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;
            cameraTransform.localPosition = startPosition + new Vector3(offset.x, offset.y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = startPosition;
        shakeCoroutine = null;
    }
}
