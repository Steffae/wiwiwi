using UnityEngine;

public class MenuSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;
    private IGameStateService gameStateService;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;

    void Start()
    {
        var gameEntrypoint = GameEntrypoint.Instance;

        audioService = gameEntrypoint.AudioService;
        gameStateService = gameEntrypoint.GameStateService;

        if (menuMusic != null)
            audioService.PlayMusic(menuMusic);

        InjectServicesIntoScene();
    }

    private void InjectServicesIntoScene()
    {
        var mainMenu = FindFirstObjectByType<MainMenu>();
        if (mainMenu != null)
        {
            mainMenu.Initialize(audioService, gameStateService);
        }

        var volumeSliders = FindObjectsByType<VolumeSlider>(FindObjectsSortMode.None);
        foreach (var slider in volumeSliders)
        {
            slider.Initialize(audioService);
        }
    }
}