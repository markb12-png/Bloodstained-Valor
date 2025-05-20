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
    public AudioClip cancelSound;  // Sound when cancel is used
    private AudioSource audioSource;

    private int selectedIndex = 0;
    private bool isLocked = false;
    private float inputCooldown = 0.2f; // Prevents fast repeated input
    private float lastInputTime;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        lastInputTime = -inputCooldown;
    }

    private void Update()
    {
        if (isLocked)
            return;

        // If a menu is open, allow cancel key to close it
        if (loadGameMenu.activeSelf || (menuFunctions != null && menuFunctions.settingsPanel.activeSelf))
        {
            CheckForCancelInput();
            return;
        }

        // Combine keyboard + D-pad input
        float vertical = Input.GetAxisRaw("Select") + Input.GetAxisRaw("SelectDpad");

        // Move selector up/down (with cooldown)
        if (Time.time - lastInputTime > inputCooldown)
        {
            if (vertical < -0.5f)
            {
                selectedIndex = (selectedIndex + 1) % menuButtons.Length;
                UpdateSelectorPosition();
                PlaySound(moveSound);
                lastInputTime = Time.time;
            }
            else if (vertical > 0.5f)
            {
                selectedIndex = (selectedIndex - 1 + menuButtons.Length) % menuButtons.Length;
                UpdateSelectorPosition();
                PlaySound(moveSound);
                lastInputTime = Time.time;
            }
        }

        // Confirm selection
        if (Input.GetButtonDown("Confirm"))
        {
            isLocked = true;
            PlaySound(selectSound);
            StartCoroutine(TriggerHighlightEffect());
        }
    }

    private void UpdateSelectorPosition()
    {
        selector.position = menuButtons[selectedIndex].transform.position;
    }

    private IEnumerator TriggerHighlightEffect()
    {
        highlightTexts[selectedIndex].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        highlightTexts[selectedIndex].gameObject.SetActive(false);
        ExecuteMenuFunction();
        isLocked = false;
    }

    private void ExecuteMenuFunction()
    {
        switch (selectedIndex)
        {
            case 0:
                menuFunctions.StartNewGame();
                break;
            case 1:
                menuFunctions.OpenSettings();
                break;
            case 2:
                menuFunctions.QuitGame();
                break;
        }
    }

    private void CheckForCancelInput()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PlaySound(cancelSound);

            if (loadGameMenu.activeSelf)
            {
                loadGameMenu.SetActive(false);
            }
            else if (menuFunctions != null && menuFunctions.settingsPanel.activeSelf)
            {
                menuFunctions.CloseSettings();
            }
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
