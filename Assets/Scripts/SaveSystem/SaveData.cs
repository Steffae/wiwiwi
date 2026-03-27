using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // Данные игрока — используем отдельные поля для координат
    public float playerX;
    public float playerY;
    public float playerZ;
    public float playerHealth;
    public float playerMaxHealth;

    // Данные врагов
    public List<EnemySaveData> enemies = new List<EnemySaveData>();

    // Время сохранения
    public string saveTime;

    // Удобные методы для работы с Vector3
    public Vector3 PlayerPosition
    {
        get => new Vector3(playerX, playerY, playerZ);
        set
        {
            playerX = value.x;
            playerY = value.y;
            playerZ = value.z;
        }
    }
}

[Serializable]
public class EnemySaveData
{
    public string enemyId;
    public string enemyType;
    public float posX;
    public float posY;
    public float posZ;
    public float health;
    public float maxHealth;
    public bool isAlive;

    public Vector3 Position
    {
        get => new Vector3(posX, posY, posZ);
        set
        {
            posX = value.x;
            posY = value.y;
            posZ = value.z;
        }
    }
}