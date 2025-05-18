using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public GameObject vfxPrefab;
    public Transform spawnPoint;

    // This method will be called by an animation event
    public void SpawnVFX()
    {
        if (vfxPrefab != null && spawnPoint != null)
        {
            Instantiate(vfxPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
