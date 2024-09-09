using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    public GameObject PausePanel;

    private void Start()
    {
        PausePanel.SetActive(false);
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
}
