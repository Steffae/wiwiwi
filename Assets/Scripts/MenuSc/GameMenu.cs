using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public GameObject GameOverPanel;

    private IGameStateService gameStateService;
    private SoundManager soundManager;

    private void Start()
    {
        GameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameStateService = GameBootstrapper.Instance.GameStateService;
        soundManager = GameBootstrapper.Instance.SoundManager;
    }

    public void AgainPlayPressed()
    {
        soundManager.PlayButtonClick();
        gameStateService.LoadLocation();
    }

    public void LvPressed()
    {
        soundManager.PlayButtonClick();
        gameStateService.LoadMenu();
    }
}