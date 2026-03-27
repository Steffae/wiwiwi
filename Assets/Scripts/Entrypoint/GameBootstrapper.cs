using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public static GameBootstrapper Instance { get; private set; }

    public IGameStateService GameStateService { get; private set; }
    public SoundManager SoundManager { get; private set; }

    [SerializeField] private SoundManager soundManagerPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        GameStateService = new GameStateService();

        // создаём SoundManager из prefab
        SoundManager = Instantiate(soundManagerPrefab);
        DontDestroyOnLoad(SoundManager.gameObject);
    }
}