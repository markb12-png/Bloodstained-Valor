using UnityEngine;

public class SceneMusicPlayer : MonoBehaviour
{
    public AudioClip sceneMusic;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = sceneMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        audioSource.Play();
    }
}
