using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject GameOverPanel;

    private void Start()
    {
        GameOverPanel.SetActive(false);
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
