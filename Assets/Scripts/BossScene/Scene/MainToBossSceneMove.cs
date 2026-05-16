using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Data;

public class MainToBossSceneMove : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private string spawnPointName = "BossSpawnPoint";

    [Header("Player Data")]
    [SerializeField] private PlayerRuntimeData playerData;

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return;

        if (other.CompareTag("Player"))
        {
            SavePlayerState(other.gameObject);
            isTransitioning = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneLoader.LoadBossLocation();
        }
    }

    private void SavePlayerState(GameObject player)
    {
        if (playerData == null)
        {
            Debug.LogWarning("PlayerRuntimeData is not assigned!");
            return;
        }

        HealthComponent health = player.GetComponent<HealthComponent>();
        if (health != null)
        {
            playerData.currentHealth = health.CurrentHealth;
            playerData.maxHealth = health.MaxHealth;
        }

        playerData.lastPosition = player.transform.position;
        playerData.lastSceneName = SceneManager.GetActiveScene().name;
        playerData.isInitialized = true;

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            playerData.physicalDamage = controller.physicalDamage;
            playerData.magicDamage = controller.magicDamage;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            TeleportPlayerToSpawnPoint(player);
        }

        isTransitioning = false;
    }

    private void TeleportPlayerToSpawnPoint(GameObject player)
    {
        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (spawnPoint != null)
        {
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
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}