using UnityEngine;
using System.IO;

public class GameSaver
{
    private string savePath;

    public GameSaver()
    {
        savePath = Application.persistentDataPath + "/save.json";
    }

    public void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Сохранено в: {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка сохранения: {e.Message}");
        }
    }

    public SaveData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("Файл сохранения не найден");
            return null;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"Загружено из: {savePath}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки: {e.Message}");
            return null;
        }
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Сохранение удалено");
        }
    }
}