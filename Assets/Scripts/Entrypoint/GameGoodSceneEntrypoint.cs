using UnityEngine;

public class GameGoodSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;
    private IGameStateService gameStateService;

    [SerializeField] private AudioClip gameGoodMusic;

    void Start()
    {
        var gameEntrypoint = GameEntrypoint.Instance;

        audioService = gameEntrypoint.AudioService;
        gameStateService = gameEntrypoint.GameStateService;

        if (gameGoodMusic != null)
            audioService.PlayMusic(gameGoodMusic);

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