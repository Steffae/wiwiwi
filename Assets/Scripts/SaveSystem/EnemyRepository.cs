using System.Collections.Generic;
using UnityEngine;

public class EnemyRepository : IRepository<List<EnemySaveData>>
{
    private List<EnemyBase> enemies = new List<EnemyBase>();
    private List<EnemySaveData> cachedData;

    public EnemyRepository()
    {
        RefreshEnemies();
    }

    public void RefreshEnemies()
    {
        enemies.Clear();
        enemies.AddRange(Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None));
    }

    public List<EnemySaveData> GetData()
    {
        RefreshEnemies();
        var data = new List<EnemySaveData>();

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            var enemyData = new EnemySaveData
            {
                enemyId = enemy.gameObject.GetInstanceID().ToString(),
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

    public void SaveData(List<EnemySaveData> data)
    {
        cachedData = data;
    }

    public void Reset()
    {
        cachedData = null;
    }

    public void Restore(List<EnemySaveData> data)
    {
        if (data == null) return;

        RefreshEnemies();

        Dictionary<string, EnemySaveData> saveDataMap = new Dictionary<string, EnemySaveData>();
        foreach (var enemyData in data)
        {
            saveDataMap[enemyData.enemyId] = enemyData;
        }

        foreach (var enemy in enemies)
        {
            string enemyId = enemy.gameObject.GetInstanceID().ToString();
            if (saveDataMap.TryGetValue(enemyId, out EnemySaveData savedData))
            {
                enemy.transform.position = savedData.Position;

                if (savedData.isAlive)
                {
                    enemy.SetHealth(savedData.health);
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