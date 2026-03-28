using UnityEngine;
using System.IO;

public class JsonSaveRepository : ISaveRepository
{
    private readonly string savePath;

    public JsonSaveRepository()
    {
        savePath = Application.persistentDataPath + "/save.json";
    }

    public void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public SaveData Load()
    {
        if (!File.Exists(savePath))
            return null;

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
    }
}