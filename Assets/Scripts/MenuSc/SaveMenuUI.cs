using UnityEngine;

public class SaveMenuUI : MonoBehaviour
{
    private ISaveInteractor saveInteractor;
    private IAudioService audioService;
    private IGameStateService gameStateService;

    [SerializeField] private AudioClip buttonClick;

    public void Initialize(ISaveInteractor interactor, IAudioService audioService, IGameStateService gameStateService = null)
    {
        this.saveInteractor = interactor;
        this.audioService = audioService;
        this.gameStateService = gameStateService ?? GameEntrypoint.Instance.GameStateService;
    }

    public void OnSavePressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        saveInteractor?.SaveGame();
    }

    public void OnLoadPressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        saveInteractor?.LoadGame();
    }

    public void OnMenuPressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        gameStateService?.LoadMenu();
    }
}