using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Data;

public class PlayerSceneTransition : MonoBehaviour
{ 
    [Header("Spawn Point in Target Scene")]
    [SerializeField] private string spawnPointName = "PlayerSpawnPoint";

    [Header("Data")]
    [SerializeField] private PlayerRuntimeData playerData;

    private bool isTransitioning = false;
    private IGameStateService gameStateService;

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return;

        if (other.CompareTag("Player"))
        {
            // Сохраняем состояние игрока перед переходом
            SavePlayerState(other.gameObject);

            // Загружаем новую сцену
            isTransitioning = true;
            gameStateService?.LoadLocationBoss();

            // Подписываемся на событие загрузки сцены для телепортации игрока
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void SavePlayerState(GameObject player)
    {
        if (playerData == null) return;

        HealthComponent health = player.GetComponent<HealthComponent>();
        if (health != null)
        {
            playerData.currentHealth = health.CurrentHealth;
        }

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            playerData.physicalDamage = controller.physicalDamage;
            playerData.magicDamage = controller.magicDamage;
        }

        playerData.lastSceneName = SceneManager.GetActiveScene().name;
        playerData.lastPosition = player.transform.position;
        playerData.isInitialized = true;

        Debug.Log($"Player state saved: HP={playerData.currentHealth}, Scene={playerData.lastSceneName}");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Находим игрока в новой сцене
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            TeleportPlayerToSpawnPoint(player);
        }

        isTransitioning = false;
    }

    private void TeleportPlayerToSpawnPoint(GameObject player)
    {
        // Ищем точку спавна по имени
        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (spawnPoint != null)
        {
            // Отключаем контроллер персонажа на время телепортации
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
                cc.enabled = true;
            }
            else
            {
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
            }

            Debug.Log($"Player teleported to spawn point: {spawnPointName}");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}