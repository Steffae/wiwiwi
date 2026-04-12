using UnityEngine;

public class GameOverSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;
    private IGameStateService gameStateService;

    [SerializeField] private AudioClip gameOverMusic;

    void Start()
    {
        var gameEntrypoint = GameEntrypoint.Instance;

        audioService = gameEntrypoint.AudioService;
        gameStateService = gameEntrypoint.GameStateService;

        if (gameOverMusic != null)
            audioService.PlayMusic(gameOverMusic);

        InjectServicesIntoScene();
    }

    private void InjectServicesIntoScene()
    {
        var gameMenu = FindFirstObjectByType<GameMenu>();
        if (gameMenu != null)
        {
            gameMenu.Initialize(audioService, gameStateService);
        }

        var volumeSliders = FindObjectsByType<VolumeSlider>(FindObjectsSortMode.None);
        foreach (var slider in volumeSliders)
        {
            slider.Initialize(audioService);
        }
    }
}