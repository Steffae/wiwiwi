using Game.Data;

public class GameStateService : IGameStateService
{
    private readonly PlayerRuntimeData playerData;
    private readonly IGameScoreService scoreService;

    public GameStateService(PlayerRuntimeData playerData, IGameScoreService scoreService)
    {
        this.playerData = playerData;
        this.scoreService = scoreService;
    }

    public void ResetPlayerData()
    {
        playerData?.FullReset();
    }

    public void ResetScoreData()
    {
        scoreService?.Reset();
    }

    public void FullReset()
    {
        ResetPlayerData();
        ResetScoreData();
    }

    public void LoadLocation() => SceneLoader.LoadLocation();
    public void LoadLocationBoss() => SceneLoader.LoadBossLocation();
    public void LoadMenu() => SceneLoader.LoadMenu();
    public void LoadEnd() => SceneLoader.LoadEnd();
    public void LoadGoodEnd() => SceneLoader.LoadGoodEnd();
}