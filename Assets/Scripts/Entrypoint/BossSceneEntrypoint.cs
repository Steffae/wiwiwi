using UnityEngine;

public class BossSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;
    private IGameStateService gameStateService;
    private IGameScoreService scoreService;

    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    void Start()
    {
        var gameEntrypoint = GameEntrypoint.Instance;

        audioService = gameEntrypoint.AudioService;
        gameStateService = gameEntrypoint.GameStateService;
        scoreService = gameEntrypoint.GameScoreService;

        if (bossMusic != null)
            audioService.PlayMusic(bossMusic);

        TrySpawnBoss();
        InjectServicesIntoScene();


    }

    private void TrySpawnBoss()
    {
        if (scoreService != null && scoreService.KillCount >= scoreService.BossSpawnKills)
        {
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab != null)
        {
            Vector3 spawnPos = bossSpawnPoint != null
                ? bossSpawnPoint.position
                : Vector3.zero;

            Quaternion spawnRot = bossSpawnPoint != null
                ? bossSpawnPoint.rotation
                : Quaternion.identity;

            Instantiate(bossPrefab, spawnPos, spawnRot);
            Debug.Log("Boss spawned!");
        }
        else
        {
            Debug.LogWarning("Boss prefab not assigned in BossSceneEntrypoint");
        }
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