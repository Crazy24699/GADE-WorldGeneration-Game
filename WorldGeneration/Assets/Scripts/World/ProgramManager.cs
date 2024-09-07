using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProgramManager : MonoBehaviour
{

    public static ProgramManager ProgramManagerInstance;
    public bool GamePaused = false;
    public bool DevMode;

    public bool StartSpawning;


    public UnityEvent SpawnWave;
    public List<GameObject> EnemyCount;

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
        //SpawnWave.Invoke();
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnWave.Invoke();
        }

    }

}
