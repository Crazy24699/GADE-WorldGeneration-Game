using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{

    public static ProgramManager ProgramManagerInstance;
    public bool GamePaused = false;

    public bool DevMode;

    public void Start()
    {
        Time.timeScale = 1.0f;
        if(ProgramManagerInstance == null)
        {
            ProgramManagerInstance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(ProgramManagerInstance);
    }


    private void Update()
    {
        switch (Time.timeScale)
        {
            case <= 0:
                GamePaused = true;
                break;

            case >0:
                GamePaused = false;
                break;
        }
    }

}
