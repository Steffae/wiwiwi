using System.Collections.Generic;
using UnityEngine;

public class GameplaySceneEntrypoint : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip victoryMusic;

    private IAudioService audioService;
    private IGameStateService gameStateService;
    private IGameScoreService scoreService;

    void Start()
    {
        var gameEntrypoint = GameEntrypoint.Instance;

        audioService = gameEntrypoint.AudioService;
        gameStateService = gameEntrypoint.GameStateService;
        scoreService = gameEntrypoint.GameScoreService;

        if (gameplayMusic != null)
            audioService.PlayMusic(gameplayMusic);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        var enemies = new List<EnemyBase>(
            FindObjectsByType<EnemyBase>(FindObjectsSortMode.None));

        var playerRepo = new PlayerRepository(player);
        var enemyRepo = new EnemyRepository();

        var saveInteractor = new SaveInteractor(
            playerRepo,
            enemyRepo,
            gameEntrypoint.SaveRepository);

        gameEntrypoint.SaveInteractor = saveInteractor;

        SubscribeToScoreEvents();
        InjectServicesIntoScene(saveInteractor);
    }

    private void SubscribeToScoreEvents()
    {
        if (scoreService != null && victoryMusic != null)
        {
            scoreService.OnVictory += () =>
            {
                audioService.PlaySoundEffect(victoryMusic);
            };
        }
    }

    private void InjectServicesIntoScene(ISaveInteractor saveInteractor)
    {
        var saveUI = FindFirstObjectByType<SaveMenuUI>();
        if (saveUI != null)
        {
            saveUI.Initialize(saveInteractor, audioService, gameStateService);
        }

        var scoreboardUI = FindFirstObjectByType<ScoreboardUI>();
        if (scoreboardUI != null)
        {
            scoreboardUI.Initialize(scoreService);
        }

        var volumeSliders = FindObjectsByType<VolumeSlider>(FindObjectsSortMode.None);
        foreach (var slider in volumeSliders)
        {
            slider.Initialize(audioService);
        }
    }

    private void OnDestroy()
    {
    }
}