using Game.Data;
using UnityEngine.SceneManagement;

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
        if (playerData != null)
        {
            playerData.FullReset();
        }
    }

    public void ResetScoreData()
    {
        if (scoreService != null)
        {
            scoreService.Reset();
        }
    }

    // Метод для полного сброса (и игрок, и счёт)
    public void FullReset()
    {
        ResetPlayerData();
        ResetScoreData();
    }

    public void LoadLocation()
    {
        SceneManager.LoadScene("Location");
    }
    public void LoadLocationBoss()
    {
        SceneManager.LoadScene("Location_boss");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void LoadBossScene()
    {
        SceneManager.LoadScene("Location_boss");
    }

    public void LoadEnd()
    {
        SceneManager.LoadScene("End");
    }

    public void LoadGoodEnd()
    {
        SceneManager.LoadScene("GoodEnd");
    }
}