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
        return new PlayerData
        {
            position = player.transform.position,
            health = healthComponent.CurrentHealth,
            maxHealth = healthComponent.MaxHealthValue
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