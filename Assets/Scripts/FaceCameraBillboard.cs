using UnityEngine;

public class FaceCameraBillboard : MonoBehaviour
{
    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = originalRotation;
    }
}
