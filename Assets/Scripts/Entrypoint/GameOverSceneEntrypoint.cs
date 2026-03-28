using UnityEngine;

public class GameOverSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;

    [SerializeField] private AudioClip gameOverMusic;

    void Start()
    {
        audioService = GameEntrypoint.Instance.AudioService;

        if (gameOverMusic != null)
            audioService.PlayMusic(gameOverMusic);
    }
}