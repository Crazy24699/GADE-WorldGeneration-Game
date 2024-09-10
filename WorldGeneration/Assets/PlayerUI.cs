using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject DefeatPanel;

    private void Start()
    {
        PausePanel.SetActive(false);
        DefeatPanel.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
        PausePanel.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        PausePanel.SetActive(false);
        
    }

    public void MainMenuTransition()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1.0f;
    }

    public void Defeat()
    {
        Time.timeScale = 0.0f;
        DefeatPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Retry()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Main Menu");
    }

}
