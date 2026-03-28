using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject GameOverPanel;

    private IAudioService audioService;

    [SerializeField] private AudioClip buttonClick;

    private void Start()
    {
        audioService = GameEntrypoint.Instance.AudioService;

        GameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AgainPlayPressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        SceneManager.LoadScene("Location");
    }

    public void LvPressed()
    {
        audioService.PlaySoundEffect(buttonClick);
        SceneManager.LoadScene("MenuScene");
    }
}