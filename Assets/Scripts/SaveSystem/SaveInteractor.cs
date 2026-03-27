using System.Collections.Generic;
using UnityEngine;

public class SaveInteractor
{
    private PlayerRepository playerRepo;
    private EnemyRepository enemyRepo;
    private GameSaver gameSaver;

    public SaveInteractor()
    {
        playerRepo = new PlayerRepository();
        enemyRepo = new EnemyRepository();
        gameSaver = new GameSaver();
    }

    public void SaveGame()
    {
        Debug.Log("Сохраняем игру...");

        PlayerData playerData = playerRepo.GetData();
        List<EnemySaveData> enemiesData = enemyRepo.GetData();

        playerRepo.SaveData(playerData);
        enemyRepo.SaveData(enemiesData);

        SaveData saveData = new SaveData();
        saveData.PlayerPosition = playerData.position;
        saveData.playerHealth = playerData.health;
        saveData.playerMaxHealth = playerData.maxHealth;
        saveData.enemies = enemiesData;
        saveData.saveTime = System.DateTime.Now.ToString("HH:mm:ss");

        gameSaver.Save(saveData);

        Debug.Log($"Игра сохранена! Позиция игрока: {playerData.position}");
    }

    public void LoadGame()
    {
        Debug.Log("Загружаем игру...");

        SaveData saveData = gameSaver.Load();

        if (saveData == null)
        {
            Debug.LogWarning("Нет сохранений!");
            return;
        }

        PlayerData playerData = new PlayerData
        {
            position = saveData.PlayerPosition,
            health = saveData.playerHealth,
            maxHealth = saveData.playerMaxHealth
        };

        playerRepo.Restore(playerData);
        enemyRepo.Restore(saveData.enemies);

        Debug.Log($"Игра загружена! Позиция игрока: {saveData.PlayerPosition}");
    }

    public bool HasSave()
    {
        return gameSaver.HasSave();
    }
}