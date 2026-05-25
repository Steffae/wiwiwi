using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveInteractor : ISaveInteractor
{
    private readonly IRepository<PlayerData> playerRepository;
    private readonly IRepository<List<EnemySaveData>> enemyRepository;
    private readonly string savePath;

    public SaveInteractor(
        IRepository<PlayerData> playerRepository,
        IRepository<List<EnemySaveData>> enemyRepository)
    {
        this.playerRepository = playerRepository;
        this.enemyRepository = enemyRepository;
        this.savePath = Application.persistentDataPath + "/save.json";
    }

    public void SaveGame()
    {
        PlayerData playerData = playerRepository.GetData();
        List<EnemySaveData> enemiesData = enemyRepository.GetData();

        SaveData saveData = new SaveData();
        saveData.PlayerPosition = playerData.position;
        saveData.playerHealth = playerData.health;
        saveData.playerMaxHealth = playerData.maxHealth;
        saveData.killCount = playerData.killCount;
        saveData.enemies = enemiesData;
        saveData.saveTime = System.DateTime.Now.ToString("HH:mm:ss");

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
            return;

        string json = File.ReadAllText(savePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        if (saveData == null)
            return;

        PlayerData playerData = new PlayerData
        {
            position = saveData.PlayerPosition,
            health = saveData.playerHealth,
            maxHealth = saveData.playerMaxHealth,
            killCount = saveData.killCount
        };

        playerRepository.Restore(playerData);
        enemyRepository.Restore(saveData.enemies);

        if (GameEntrypoint.Instance?.GameScoreService != null)
        {
            GameEntrypoint.Instance.GameScoreService.Restore(saveData.killCount);
        }
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }
}
