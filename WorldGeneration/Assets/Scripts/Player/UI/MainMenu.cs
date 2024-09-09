using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject OptionsMenuObject;
    public GameObject MainMenuObject;

    private void Start()
    {
        Time.timeScale = 1.0f;
    }

    public void PlayGame()
    {
        //SceneManager.UnloadScene("Restored"); 
        
        SceneManager.LoadScene("Restored"); 
        
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void ShowOptions()
    {
        MainMenuObject.SetActive(false);
    }
}
