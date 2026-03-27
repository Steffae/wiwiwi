using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject settingsPanel;
    public GameObject startPanel;

    private IGameStateService gameStateService;
    private SoundManager soundManager;

    private void Start()
    {
        settingsPanel.SetActive(false);
        startPanel.SetActive(false);
        menuPanel.SetActive(true);

        // Получаем сервисы из Bootstrapper
        gameStateService = GameBootstrapper.Instance.GameStateService;
        soundManager = GameBootstrapper.Instance.SoundManager;
    }

    public void PlayPressed()
    {
        soundManager.PlayButtonClick();
        menuPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public void SettingsPressed()
    {
        soundManager.PlayButtonClick();
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ExitPressed()
    {
        soundManager.PlayButtonClick();
        Application.Quit();
    }

    public void BackPressed()
    {
        soundManager.PlayButtonClick();
        settingsPanel.SetActive(false);
        startPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void LvPressed()
    {
        soundManager.PlayButtonClick();
        gameStateService.LoadLocation();   // ← ВАЖНО: теперь через сервис
    }
}