using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject settingsPanel;
    public GameObject startPanel;

    private MenuSceneEntrypoint sceneEntrypoint;
    private IAudioService audioService;

    [SerializeField] private AudioClip buttonClick;

    private void Start()
    {
        sceneEntrypoint = FindFirstObjectByType<MenuSceneEntrypoint>();
        audioService = GameEntrypoint.Instance.AudioService;

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
        sceneEntrypoint.ExitGame();
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
        sceneEntrypoint.LoadLocation();
    }
}