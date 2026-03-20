using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject GameOverPanel;

    private void Start()
    {
        GameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AgainPlayPressed()
    {
        SceneManager.LoadScene("Location");
    }

    public void LvPressed()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
