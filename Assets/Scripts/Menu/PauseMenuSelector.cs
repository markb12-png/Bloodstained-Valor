using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PauseMenuSelector : MonoBehaviour
{
    public RectTransform selector;
    public Button[] pauseButtons;
    public TextMeshProUGUI[] highlightTexts;
    public PauseManager pauseManager;

    [Header("Audio")]
    public AudioClip moveSound;
    public AudioClip selectSound;
    public AudioClip cancelSound;
    private AudioSource audioSource;

    private int selectedIndex = 0;
    private bool isLocked = false;
    private float inputCooldown = 0.2f;
    private float lastInputTime;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        lastInputTime = -inputCooldown;
        ResetSelector();
    }

    private void Update()
    {
        if (!pauseManager || !pauseManager.pausePanel.activeSelf || isLocked)
            return;

        float vertical = Input.GetAxisRaw("Select") + Input.GetAxisRaw("SelectDpad");

        if (Time.time - lastInputTime > inputCooldown)
        {
            if (vertical < -0.5f)
            {
                selectedIndex = (selectedIndex + 1) % pauseButtons.Length;
                UpdateSelectorPosition();
                PlaySound(moveSound);
                lastInputTime = Time.time;
            }
            else if (vertical > 0.5f)
            {
                selectedIndex = (selectedIndex - 1 + pauseButtons.Length) % pauseButtons.Length;
                UpdateSelectorPosition();
                PlaySound(moveSound);
                lastInputTime = Time.time;
            }
        }

        if (Input.GetButtonDown("Confirm"))
        {
            isLocked = true;
            PlaySound(selectSound);
            StartCoroutine(TriggerHighlightEffect());
        }

        if (Input.GetButtonDown("Cancel"))
        {
            PlaySound(cancelSound);
            if (pauseManager.settingsPanel.activeSelf)
                pauseManager.CloseSettings();
            else
                pauseManager.Resume();
        }
    }

    private void UpdateSelectorPosition()
    {
        selector.position = pauseButtons[selectedIndex].transform.position;
    }

    private IEnumerator TriggerHighlightEffect()
    {
        if (highlightTexts.Length > selectedIndex)
            highlightTexts[selectedIndex].gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (highlightTexts.Length > selectedIndex)
            highlightTexts[selectedIndex].gameObject.SetActive(false);

        ExecuteMenuFunction();
        isLocked = false;
    }

    private void ExecuteMenuFunction()
    {
        switch (selectedIndex)
        {
            case 0:
                pauseManager.Resume();
                break;
            case 1:
                pauseManager.OpenSettings();
                break;
            case 2:
                pauseManager.QuitToTitle();
                break;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void ResetSelector()
    {
        selectedIndex = 0;
        isLocked = false;
        lastInputTime = -inputCooldown;
        UpdateSelectorPosition();
    }
}
