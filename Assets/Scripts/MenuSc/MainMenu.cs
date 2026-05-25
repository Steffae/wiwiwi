using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject settingsPanel;
    public GameObject startPanel;

    private IAudioService audioService;
    private IGameStateService gameStateService;

    [SerializeField] private AudioClip buttonClick;

    public void Initialize(IAudioService audioService, IGameStateService gameStateService)
    {
        this.audioService = audioService;
        this.gameStateService = gameStateService;

        settingsPanel.SetActive(false);
        startPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void PlayPressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        menuPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public void SettingsPressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ExitPressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        Application.Quit();
    }

    public void BackPressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        settingsPanel.SetActive(false);
        startPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void LvPressed()
    {
        audioService.PlaySoundEffect(buttonClick);

        // Сбрасываем данные игрока перед загрузкой локации
        gameStateService.FullReset();

        gameStateService.LoadLocation();
    }
}