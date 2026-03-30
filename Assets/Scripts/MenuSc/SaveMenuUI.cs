using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveMenuUI : MonoBehaviour
{
    private ISaveInteractor saveInteractor;
    private IAudioService audioService;

    [SerializeField] private AudioClip buttonClick;

    public void Initialize(ISaveInteractor interactor)
    {
        saveInteractor = interactor;
        audioService = GameEntrypoint.Instance.AudioService;
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
        SceneManager.LoadScene("MenuScene");
    }
}