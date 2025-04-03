using UnityEngine;

public class TogglePauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas; // Reference to the Canvas GameObject

    private bool isPaused = false; // Tracks the state of the Canvas

    void Start()
    {
        // Ensure the Canvas starts off as disabled
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(true); // Enable the Canvas
        }
        Time.timeScale = 0f; // Freeze the game
        isPaused = true;
    }

    public void Resume()
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false); // Disable the Canvas
        }
        Time.timeScale = 1f; // Resume the game
        isPaused = false;
    }
}
