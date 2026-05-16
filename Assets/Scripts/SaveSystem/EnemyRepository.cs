using System.Collections.Generic;
using UnityEngine;

public class EnemyRepository : IRepository<List<EnemySaveData>>
{
    private List<EnemySaveData> cachedData;

    public EnemyRepository()
    {
    }

    public List<EnemySaveData> GetData()
    {
        var data = new List<EnemySaveData>();

        // ������� ���� ������ �� �����
        EnemyBase[] enemies = Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            var enemyData = new EnemySaveData();
            enemyData.Position = enemy.transform.position;
            enemyData.health = enemy.CurrentHealth;
            enemyData.maxHealth = enemy.MaxHealth;
            enemyData.isAlive = !enemy.IsDying && enemy.CurrentHealth > 0;
            enemyData.enemyId = enemy.gameObject.GetInstanceID().ToString();

            // ���������� ��� ����� � ��� �����
            if (enemy is MeleeEnemy melee)
            {
                enemyData.enemyType = "Melee";
                enemyData.attackType = melee.GetAttackType().ToString();
            }
            else if (enemy is RangedEnemy ranged)
            {
                enemyData.enemyType = "Ranged";
                enemyData.attackType = ranged.GetAttackType().ToString();
            }

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

        // ������� ���� ������������ ������
        EnemyBase[] existingEnemies = Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (var enemy in existingEnemies)
        {
            Object.Destroy(enemy.gameObject);
        }

        // ������ ������ ������
        foreach (var enemyData in data)
        {
            SpawnEnemyFromData(enemyData);
        }
    }

    private void SpawnEnemyFromData(EnemySaveData data)
    {
        GameObject prefab = GetPrefabByType(data.enemyType, data.attackType);
        if (prefab == null)
        {
            Debug.LogWarning($"�� ������ ������ ��� {data.enemyType} � ������ {data.attackType}");
            return;
        }

        GameObject enemyObj = Object.Instantiate(prefab, data.Position, Quaternion.identity);

        // ��������������� ��� �����
        if (enemyObj.TryGetComponent<MeleeEnemy>(out var melee))
        {
            if (System.Enum.TryParse<MeleeAttackType>(data.attackType, out var attackType))
            {
                melee.SetAttackType(attackType);
            }
            melee.SetHealth(data.health);
        }
        else if (enemyObj.TryGetComponent<RangedEnemy>(out var ranged))
        {
            if (System.Enum.TryParse<RangedAttackType>(data.attackType, out var attackType))
            {
                ranged.SetAttackType(attackType);
            }
            ranged.SetHealth(data.health);
        }

        if (!data.isAlive)
        {
            Object.Destroy(enemyObj);
        }
    }

    private GameObject GetPrefabByType(string enemyType, string attackType)
    {
        // ��������� ������� �� Resources ��� ������� �� � �����
        // ����� ��������� ���� � ��������
        string path = "";

        if (enemyType == "Melee")
        {
            path = attackType == "Push" ? "Prefabs/Enemies/MeleeEnemy_Push" : "Prefabs/Enemies/MeleeEnemy_Jump";
        }
        else if (enemyType == "Ranged")
        {
            path = attackType == "Bird" ? "Prefabs/Enemies/RangedEnemy_Bird" : "Prefabs/Enemies/RangedEnemy_Octopus";
        }

        return Resources.Load<GameObject>(path);
    }
}