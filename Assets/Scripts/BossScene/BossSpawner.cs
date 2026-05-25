using UnityEngine;

namespace Game.Boss
{
    public class BossSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private Transform spawnPoint;

        private IGameScoreService scoreService;
        private bool hasSpawned = false;

        private void Start()
        {
            var gameEntrypoint = GameEntrypoint.Instance;
            scoreService = gameEntrypoint?.GameScoreService;

            if (scoreService != null)
            {
                scoreService.OnBossShouldSpawn += SpawnBoss;

                if (scoreService.KillCount >= scoreService.BossSpawnKills)
                {
                    SpawnBoss();
                }
            }
        }

        private void OnDestroy()
        {
            if (scoreService != null)
            {
                scoreService.OnBossShouldSpawn -= SpawnBoss;
            }
        }

        public void SpawnBoss()
        {
            if (hasSpawned || bossPrefab == null) return;
            hasSpawned = true;

            Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

            Instantiate(bossPrefab, position, rotation);
        }
    }
}