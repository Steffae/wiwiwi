using System.Collections.Generic;
using UnityEngine;

public class SaveInteractor : ISaveInteractor
{
    private readonly IRepository<PlayerData> playerRepository;
    private readonly IRepository<List<EnemySaveData>> enemyRepository;
    private readonly ISaveRepository saveRepository;

    public SaveInteractor(
        IRepository<PlayerData> playerRepository,
        IRepository<List<EnemySaveData>> enemyRepository,
        ISaveRepository saveRepository)
    {
        this.playerRepository = playerRepository;
        this.enemyRepository = enemyRepository;
        this.saveRepository = saveRepository;
    }

    public void SaveGame()
    {
        PlayerData playerData = playerRepository.GetData();
        List<EnemySaveData> enemiesData = enemyRepository.GetData();

        SaveData saveData = new SaveData();
        saveData.PlayerPosition = playerData.position;
        saveData.playerHealth = playerData.health;
        saveData.playerMaxHealth = playerData.maxHealth;
        saveData.enemies = enemiesData;
        saveData.saveTime = System.DateTime.Now.ToString("HH:mm:ss");

        saveRepository.Save(saveData);
    }

    public void LoadGame()
    {
        SaveData saveData = saveRepository.Load();
        if (saveData == null)
            return;

        PlayerData playerData = new PlayerData
        {
            position = saveData.PlayerPosition,
            health = saveData.playerHealth,
            maxHealth = saveData.playerMaxHealth
        };

        playerRepository.Restore(playerData);
        enemyRepository.Restore(saveData.enemies);
    }

    public bool HasSave()
    {
        return saveRepository.HasSave();
    }
}