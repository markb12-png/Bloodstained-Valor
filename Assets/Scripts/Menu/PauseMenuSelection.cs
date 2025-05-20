using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuSelection : MonoBehaviour
{
    public RectTransform selector; // Arrow indicator
    public Button[] pauseMenuButtons; // All pause menu buttons
    private int selectedIndex = 0;

    private void Update()
    {
        HandleSelectionInput();
    }

    private void HandleSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % pauseMenuButtons.Length;
            UpdateSelectorPosition();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + pauseMenuButtons.Length) % pauseMenuButtons.Length;
            UpdateSelectorPosition();
        }
    }

    private void UpdateSelectorPosition()
    {
        selector.position = pauseMenuButtons[selectedIndex].transform.position;
    }
}
