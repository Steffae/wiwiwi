using UnityEngine.SceneManagement;

public class GameStateService : IGameStateService
{
    public void LoadLocation()
    {
        SceneManager.LoadScene("Location");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void LoadEnd()
    {
        SceneManager.LoadScene("End");
    }
}