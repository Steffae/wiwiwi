using UnityEngine;

public class BossSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;
    private ISaveInteractor saveInteractor;

    [SerializeField] private AudioClip bossMusic;

    void Start()
    {
        audioService = GameEntrypoint.Instance.AudioService;

        if (bossMusic != null)
            audioService.PlayMusic(bossMusic);

        InjectSaveInteractorIntoUI();
    }

    private void InjectSaveInteractorIntoUI()
    {
        var saveUI = FindFirstObjectByType<SaveMenuUI>();
        if (saveUI != null)
        {
            saveUI.Initialize(saveInteractor);
        }
    }
}