using UnityEngine;

public class PlayerRepository : IRepository<PlayerData>
{
    private GameObject player;
    private HealthComponent healthComponent;
    private PlayerData cachedData;

    public PlayerRepository()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            healthComponent = player.GetComponent<HealthComponent>();
        }
    }

    public PlayerData GetData()
    {
        if (player == null) return null;

        return new PlayerData
        {
            position = player.transform.position,
            health = healthComponent?.CurrentHealth ?? 0,
            maxHealth = healthComponent?.MaxHealthValue ?? 100
        };
    }

    public void SaveData(PlayerData data)
    {
        cachedData = data;
    }

    public void Reset()
    {
        cachedData = null;
    }

    public void Restore(PlayerData data)
    {
        if (player == null || data == null) return;

        player.transform.position = data.position;
        if (healthComponent != null)
        {
            healthComponent.SetHealth(data.health);
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public float x;
    public float y;
    public float z;
    public float health;
    public float maxHealth;

    public Vector3 position
    {
        get => new Vector3(x, y, z);
        set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
}