using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuSelector : MonoBehaviour
{
    public RectTransform selector; // Arrow indicator
    public Button[] menuButtons;   // All menu buttons
    public TextMeshProUGUI[] highlightTexts; // Highlight effect texts
    public MenuFunctions menuFunctions;      // Reference to menu function script
    public GameObject loadGameMenu;          // Reference to Load Game UI panel

    public AudioClip moveSound;    // Sound when moving selector
    public AudioClip selectSound;  // Sound when selecting option
    private AudioSource audioSource;

    private int selectedIndex = 0;
    private bool isLocked = false; // Locks input during selection

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (isLocked || loadGameMenu.activeSelf)
        {
            CheckForEscapeKey(); // Allow Escape key to close submenus
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % menuButtons.Length;
            UpdateSelectorPosition();
            PlaySound(moveSound);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + menuButtons.Length) % menuButtons.Length;
            UpdateSelectorPosition();
            PlaySound(moveSound);
        }

        if (Input.GetKeyDown(KeyCode.Return)) // When player presses Enter
        {
            isLocked = true; // Lock input
            PlaySound(selectSound);
            StartCoroutine(TriggerHighlightEffect()); // Start highlight effect before executing action
        }
    }

    private void UpdateSelectorPosition()
    {
        selector.position = menuButtons[selectedIndex].transform.position;
    }

    private IEnumerator TriggerHighlightEffect()
    {
        highlightTexts[selectedIndex].gameObject.SetActive(true); // Show highlight effect

        yield return new WaitForSeconds(1f); // Wait for 1 second

        highlightTexts[selectedIndex].gameObject.SetActive(false); // Hide highlight effect

        ExecuteMenuFunction(); // Execute selected option
        isLocked = false;      // Unlock input after action completes
    }

    private void ExecuteMenuFunction()
    {
        switch (selectedIndex)
        {
            case 0: // New Game
                menuFunctions.StartNewGame();
                break;
            case 1: // Quit Game
                menuFunctions.QuitGame();
                break;
        }
    }

    private void CheckForEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (loadGameMenu.activeSelf)
                loadGameMenu.SetActive(false); // Close Load Game menu
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
