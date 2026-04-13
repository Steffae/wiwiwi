using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public GameObject GameOverPanel;

    private IAudioService audioService;
    private IGameStateService gameStateService;

    [SerializeField] private AudioClip buttonClick;

    public void Initialize(IAudioService audioService, IGameStateService gameStateService)
    {
        this.audioService = audioService;
        this.gameStateService = gameStateService;

        GameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AgainPlayPressed()
    {
        audioService.PlaySoundEffect(buttonClick);

        // Сбрасываем данные игрока при рестарте после смерти
        gameStateService.FullReset();

        gameStateService.LoadLocation();
    }

    public void LvPressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        gameStateService.LoadMenu();
    }
}