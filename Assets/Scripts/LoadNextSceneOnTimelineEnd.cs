using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LoadNextSceneOnTimelineEnd : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private void Start()
    {
        if (director != null)
            director.stopped += OnCutsceneEnd;
    }

    private void OnCutsceneEnd(PlayableDirector d)
    {
        SceneManager.LoadScene("Level 1");
    }
}
