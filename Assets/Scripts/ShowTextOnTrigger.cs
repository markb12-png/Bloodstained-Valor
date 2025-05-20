using UnityEngine;

public class ShowTextOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject textObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && textObject != null)
            textObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && textObject != null)
            textObject.SetActive(false);
    }
}
