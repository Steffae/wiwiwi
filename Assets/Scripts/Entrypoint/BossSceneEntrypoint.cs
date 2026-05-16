using UnityEngine;

public class BossSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;
    private IGameStateService gameStateService;

    [Header("Music")]
    [SerializeField] private AudioClip bossMusic;

    void Start()
    {
        var gameEntrypoint = GameEntrypoint.Instance;

        audioService = gameEntrypoint.AudioService;
        gameStateService = gameEntrypoint.GameStateService;

        if (bossMusic != null)
            audioService.PlayMusic(bossMusic);

        InjectServicesIntoScene();
    }

    private void InjectServicesIntoScene()
    {
        var saveUI = FindFirstObjectByType<SaveMenuUI>();
        if (saveUI != null)
        {
            saveUI.Initialize(null, audioService, gameStateService);
        }

        var volumeSliders = FindObjectsByType<VolumeSlider>(FindObjectsSortMode.None);
        foreach (var slider in volumeSliders)
        {
            slider.Initialize(audioService);
        }
    }
}