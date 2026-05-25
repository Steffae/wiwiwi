using UnityEngine;

public class GameScoreService : IGameScoreService
{
    public int KillCount { get; private set; }

    public int VictoryKills => 5;
    public int BossSpawnKills => 3;

    public event System.Action<int> OnKillCountChanged;
    public event System.Action OnBossShouldSpawn;
    public event System.Action OnVictory;

    public void OnEnemyKilled()
    {
        KillCount++;
        OnKillCountChanged?.Invoke(KillCount);

        if (KillCount == BossSpawnKills)
        {
            OnBossShouldSpawn?.Invoke();
        }

        if (KillCount == VictoryKills)
        {
            OnVictory?.Invoke();
        }
    }

    public void Reset()
    {
        KillCount = 0;
    }

    public void Restore(int killCount)
    {
        KillCount = killCount;
        OnKillCountChanged?.Invoke(KillCount);
    }
}
