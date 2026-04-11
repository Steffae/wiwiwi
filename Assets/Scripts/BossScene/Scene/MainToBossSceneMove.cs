using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Data;

public class MainToBossSceneMove : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string targetSceneName = "Location_boss";
    [SerializeField] private string triggerTag = "Player";

    [Header("Spawn Settings")]
    [SerializeField] private string spawnPointName = "BossSpawnPoint";

    [Header("Player Data")]
    [SerializeField] private PlayerRuntimeData playerData;

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return;

        if (other.CompareTag(triggerTag))
        {
            // Сохраняем состояние игрока
            SavePlayerState(other.gameObject);

            // Блокируем повторный вход
            isTransitioning = true;

            // Подписываемся на событие загрузки сцены
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Загружаем сцену
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private void SavePlayerState(GameObject player)
    {
        if (playerData == null)
        {
            Debug.LogWarning("PlayerRuntimeData is not assigned!");
            return;
        }

        // Сохраняем здоровье
        HealthComponent health = player.GetComponent<HealthComponent>();
        if (health != null)
        {
            playerData.currentHealth = health.CurrentHealth;
            playerData.maxHealth = health.MaxHealthValue;
            Debug.Log($"Saved player health: {playerData.currentHealth}/{playerData.maxHealth}");
        }

        // Сохраняем позицию и сцену
        playerData.lastPosition = player.transform.position;
        playerData.lastSceneName = SceneManager.GetActiveScene().name;
        playerData.isInitialized = true;

        // Сохраняем урон
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            playerData.physicalDamage = controller.physicalDamage;
            playerData.magicDamage = controller.magicDamage;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Отписываемся от события
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Находим игрока в новой сцене
        GameObject player = GameObject.FindGameObjectWithTag(triggerTag);
        if (player != null)
        {
            TeleportPlayerToSpawnPoint(player);
        }
        else
        {
            Debug.LogWarning($"Player with tag '{triggerTag}' not found in scene {targetSceneName}");
        }

        isTransitioning = false;
    }

    private void TeleportPlayerToSpawnPoint(GameObject player)
    {
        // Ищем точку спавна
        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (spawnPoint != null)
        {
            // Телепортируем игрока
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

            Debug.Log($"Player teleported to: {spawnPointName} at {spawnPoint.transform.position}");
        }
        else
        {
            Debug.LogWarning($"Spawn point '{spawnPointName}' not found! Player remains at default position.");
        }
    }

    private void OnDestroy()
    {
        // Очищаем подписку при уничтожении объекта
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}