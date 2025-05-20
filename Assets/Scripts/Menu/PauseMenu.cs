using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas; // Assign the Pause Menu UI Canvas
    public GameObject darkOverlay; // Assign a UI panel for darkening the background
    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze only gameplay mechanics
        darkOverlay.SetActive(true); // Enable background darkening effect
        pauseCanvas.SetActive(true); // Show the pause menu

        // Allow UI interactions to continue even when the game is paused
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume game logic
        darkOverlay.SetActive(false); // Remove dark background effect
        pauseCanvas.SetActive(false); // Hide the pause menu
    }
}
