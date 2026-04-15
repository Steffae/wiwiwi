using Game.Data;
using UnityEngine;

public class GameEntrypoint : MonoBehaviour
{
    public static GameEntrypoint Instance { get; private set; }

    public IAudioService AudioService { get; private set; }
    public IGameStateService GameStateService { get; private set; }
    public IGameScoreService GameScoreService { get; private set; }
    public ISaveInteractor SaveInteractor { get; set; }

    [Header("Player Data")]
    [SerializeField] private PlayerRuntimeData playerData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGlobalServices();
    }

    private void InitializeGlobalServices()
    {
        AudioService = new AudioService(gameObject);
        GameScoreService = new GameScoreService();
        GameStateService = new GameStateService(playerData, GameScoreService);
    }
}
