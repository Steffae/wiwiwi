using UnityEngine;

public class PlayerRepository : IRepository<PlayerData>
{
    private readonly GameObject player;
    private readonly HealthComponent healthComponent;

    public PlayerRepository(GameObject player)
    {
        this.player = player;
        healthComponent = player.GetComponent<HealthComponent>();
    }

    public PlayerData GetData()
    {
        int killCount = 0;
        if (GameEntrypoint.Instance?.GameScoreService != null)
        {
            killCount = GameEntrypoint.Instance.GameScoreService.KillCount;
        }

        return new PlayerData
        {
            position = player.transform.position,
            health = healthComponent.CurrentHealth,
            maxHealth = healthComponent.MaxHealth,
            killCount = killCount
        };
    }

    public void SaveData(PlayerData data) { }

    public void Reset() { }

    public void Restore(PlayerData data)
    {
        player.transform.position = data.position;
        healthComponent.SetHealth(data.health);
    }
}