using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ShowTextOnTrigger : MonoBehaviour
{
    public GameObject textObject;
    public Image fadeImage; // Reference to your 'fade' panel's Image component

    private bool playerInTrigger = false;
    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            if (textObject != null) textObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (textObject != null) textObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInTrigger && !isTransitioning && Input.GetButtonDown("Interact"))
        {
            StartCoroutine(FadeAndLoad());
        }
    }

    private IEnumerator FadeAndLoad()
    {
        isTransitioning = true;

        float fadeTime = 3f;
        float delayAfterFade = 2f;
        float elapsed = 0f;

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeTime);
            fadeImage.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(delayAfterFade); // Total delay = 5s

        string current = SceneManager.GetActiveScene().name;
        string next = current switch
        {
            "level 1" => "level 2",
            "level 2" => "level 3",
            "level 3" => "Ending scene",
            _ => null
        };

        if (!string.IsNullOrEmpty(next))
        {
            SceneManager.LoadScene(next);
        }
    }
}
