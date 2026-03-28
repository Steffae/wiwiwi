using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;

    void Start()
    {
        audioService = GameEntrypoint.Instance.AudioService;

        if (menuMusic != null)
            audioService.PlayMusic(menuMusic);
    }

    public void LoadLocation()
    {
        SceneManager.LoadScene("Location");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}