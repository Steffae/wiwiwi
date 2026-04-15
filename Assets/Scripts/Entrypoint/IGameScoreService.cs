public interface IGameScoreService
{
    int KillCount { get; }
    int VictoryKills { get; }
    int BossSpawnKills { get; }

    event System.Action<int> OnKillCountChanged;
    event System.Action OnBossShouldSpawn;
    event System.Action OnVictory;

    void OnEnemyKilled();
    void Reset();
    void Restore(int killCount);
}
