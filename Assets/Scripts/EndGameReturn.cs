using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameReturn : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("title screen");
        }
    }
}
