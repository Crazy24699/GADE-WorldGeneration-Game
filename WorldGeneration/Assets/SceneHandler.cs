using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SceneHandler : MonoBehaviour
{
    private HashSet<PathGenerator> PathGenerators = new HashSet<PathGenerator>();
    public static SceneHandler SceneInstance;

    public EnemySpawnerLogic SpawnerScript;

    public bool PathsGenerated = false;
    private bool GameOver = false;

    public LayerMask WalkableLayers;

    public UnityEvent WaveBeaten;
    public UnityEvent EndGame;
    public UnityEvent StartWave;

    // Start is called before the first frame update
    void Start()
    {
        PathGenerators = FindObjectsByType<PathGenerator>(FindObjectsSortMode.None).ToHashSet();
        StartCoroutine(RunWorldSetup());
        SceneInstance = this;

        SpawnerScript = FindObjectOfType<EnemySpawnerLogic>();
        InvokeRepeating(nameof(StartSpawning), 0.0f, .025f);

        EndGame.AddListener(() => HandleGameOver());
    }

    private IEnumerator RunWorldSetup()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (var GeneratorScript in PathGenerators)
        {
            GeneratorScript.HandleGeneration();
            //Debug.Log(GeneratorScript.gameObject.name);
        }
        yield return new WaitForSeconds(0.025f);
        //ProgramManager.ProgramManagerInstance.SpawnWave.Invoke();

    }

    private void HandleGameOver()
    {
        GameOver = true;
        Time.timeScale = 0.0f;
    }

    private void StartSpawning()
    {
        if(!PathsGenerated)
        {
            return;
        }
        ProgramManager.ProgramManagerInstance.SpawnWave.Invoke();
        CancelInvoke(nameof(StartSpawning));
    }

}
