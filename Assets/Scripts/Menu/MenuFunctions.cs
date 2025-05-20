using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuFunctions : MonoBehaviour
{
    public void StartNewGame()
    {
        // Corrected: Removed extra parentheses
        SceneManager.LoadScene("sword clash test");
    }

    public void QuitGame()
    {
        // Corrected: Removed extra parentheses
        Application.Quit();

#if UNITY_EDITOR
        // This stops play mode in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
