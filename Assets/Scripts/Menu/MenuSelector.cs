using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuSelector : MonoBehaviour
{
    public RectTransform selector; // Arrow indicator
    public Button[] menuButtons; // All menu buttons
    public TextMeshProUGUI[] highlightTexts; // Highlight effect texts
    public MenuFunctions menuFunctions; // Reference to menu function script
    public GameObject loadGameMenu; // Reference to Load Game UI panel
    public GameObject settingsMenu; // Reference to Settings UI panel
    private int selectedIndex = 0;
    private bool isLocked = false; // Locks input during selection

    private void Update()
    {
        if (isLocked || SubmenuIsOpen())
        {
            CheckForEscapeKey(); // Allow Escape key to close submenus
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % menuButtons.Length;
            UpdateSelectorPosition();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + menuButtons.Length) % menuButtons.Length;
            UpdateSelectorPosition();
        }

        if (Input.GetKeyDown(KeyCode.Return)) // When player presses Enter
        {
            isLocked = true; // Lock input
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
        isLocked = false; // Unlock input after action completes
    }

    private void ExecuteMenuFunction()
    {
        switch (selectedIndex)
        {
            case 0: // New Game
                menuFunctions.StartNewGame();
                break;
            case 2: // Settings
                settingsMenu.SetActive(true); // Open Settings menu
                break;
            case 3: // Quit Game
                menuFunctions.QuitGame();
                break;
        }
    }

    private bool SubmenuIsOpen()
    {
        return loadGameMenu.activeSelf || settingsMenu.activeSelf; // Check if either menu is open
    }

    private void CheckForEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (loadGameMenu.activeSelf) loadGameMenu.SetActive(false); // Close Load Game menu
            if (settingsMenu.activeSelf) settingsMenu.SetActive(false); // Close Settings menu
        }
    }
}
