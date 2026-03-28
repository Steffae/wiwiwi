using System.Collections.Generic;
using UnityEngine;

public class GameplaySceneEntrypoint : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip gameplayMusic;

    private ISaveInteractor saveInteractor;
    private IAudioService audioService;

    void Start()
    {
        var gameEntrypoint = GameEntrypoint.Instance;

        audioService = gameEntrypoint.AudioService;

        // 🎵 Музыка сцены
        if (gameplayMusic != null)
            audioService.PlayMusic(gameplayMusic);

        // Получаем объекты сцены
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        var enemies = new List<EnemyBase>(
            FindObjectsByType<EnemyBase>(FindObjectsSortMode.None));

        // Репозитории
        var playerRepo = new PlayerRepository(player);
        var enemyRepo = new EnemyRepository(enemies);

        // Interactor
        saveInteractor = new SaveInteractor(
            playerRepo,
            enemyRepo,
            gameEntrypoint.SaveRepository);

        // Передаём interactor в UI
        InjectSaveInteractorIntoUI();
    }

    private void InjectSaveInteractorIntoUI()
    {
        var saveUI = FindFirstObjectByType<SaveMenuUI>();
        if (saveUI != null)
        {
            saveUI.Initialize(saveInteractor);
        }
    }
}