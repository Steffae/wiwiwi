using UnityEngine;

public class GameEntrypoint : MonoBehaviour
{
    public static GameEntrypoint Instance { get; private set; }

    public IAudioService AudioService { get; private set; }
    public ISaveRepository SaveRepository { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeServices();
    }

    private void InitializeServices()
    {
        // Инициализация основных сервисов (внедрение зависимостей)
        AudioService = new AudioService(gameObject);
        SaveRepository = new JsonSaveRepository();
    }
}