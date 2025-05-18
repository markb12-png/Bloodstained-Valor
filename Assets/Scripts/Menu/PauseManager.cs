using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject darkOverlay;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetButtonDown("pause"))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
        darkOverlay.SetActive(isPaused);
        settingsPanel.SetActive(false);

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            var selector = pausePanel.GetComponent<PauseMenuSelector>();
            if (selector != null)
            {
                selector.enabled = true;
                selector.ResetSelector();
            }
        }
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        darkOverlay.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("title screen");
    }
}
