using UnityEngine;

public class CameraShakeRelay : MonoBehaviour
{
    [SerializeField] private CameraShake cameraShake;

    public void TriggerDefaultShake()
    {
        if (cameraShake != null)
            cameraShake.TriggerDefaultShake();
    }

    public void TriggerLightShake()
    {
        if (cameraShake != null)
            cameraShake.TriggerLightShake();
    }

    public void TriggerHeavyShake()
    {
        if (cameraShake != null)
            cameraShake.TriggerHeavyShake();
    }
}
