using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject settingsPanel;
    public GameObject startPanel;

    private void Start()
    {
        settingsPanel.SetActive(false);
        startPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
    public void PlayPressed()
    {
        menuPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public void SettingsPressed()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void ExitPressed()
    {
        // Выход из игры
        Application.Quit();
        Debug.Log("выход из игры!!!");
    }

    public void BackPressed()
    {
        settingsPanel.SetActive(false);
        startPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void LvPressed()
    {
        SceneManager.LoadScene("Location");
    }
}