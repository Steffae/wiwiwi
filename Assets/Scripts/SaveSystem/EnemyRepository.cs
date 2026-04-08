using System.Collections.Generic;
using UnityEngine;

public class EnemyRepository : IRepository<List<EnemySaveData>>
{
    private readonly List<EnemyBase> enemies;

    public EnemyRepository(List<EnemyBase> enemies)
    {
        this.enemies = enemies;
    }

    public List<EnemySaveData> GetData()
    {
        var data = new List<EnemySaveData>();

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            var enemyData = new EnemySaveData
            {
                enemyId = enemy.GetInstanceID().ToString(),
                enemyType = enemy is MeleeEnemy ? "Melee" : "Ranged",
                health = enemy.CurrentHealth,
                maxHealth = enemy.MaxHealthValue,
                isAlive = !enemy.IsDying && enemy.CurrentHealth > 0
            };

            enemyData.Position = enemy.transform.position;
            data.Add(enemyData);
        }

        return data;
    }

    public void SaveData(List<EnemySaveData> data) { }

    public void Reset() { }

    public void Restore(List<EnemySaveData> data)
    {
        var map = new Dictionary<string, EnemySaveData>();

        foreach (var enemyData in data)
            map[enemyData.enemyId] = enemyData;

        foreach (var enemy in enemies)
        {
            string id = enemy.GetInstanceID().ToString();

            if (map.TryGetValue(id, out var saved))
            {
                enemy.transform.position = saved.Position;

                if (saved.isAlive)
                {
                    enemy.SetHealth(saved.health);
                    enemy.gameObject.SetActive(true);
                }
                else
                {
                    enemy.gameObject.SetActive(false);
                }
            }
        }
    }
}