using UnityEngine;

public class IgnorePlayerColliders : MonoBehaviour
{
    private void Start()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogWarning("No Collider2D found on this GameObject.");
            return;
        }

        // Find all GameObjects tagged "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Collider2D[] playerColliders = player.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D playerCollider in playerColliders)
            {
                Physics2D.IgnoreCollision(myCollider, playerCollider);
            }
        }
    }
}
