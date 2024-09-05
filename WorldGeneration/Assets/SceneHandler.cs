using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneHandler : MonoBehaviour
{
    private HashSet<PathGenerator> PathGenerators = new HashSet<PathGenerator>();
    public static SceneHandler SceneInstance;

    public EnemySpawnerLogic SpawnerScript;

    public LayerMask WalkableLayers;

    // Start is called before the first frame update
    void Start()
    {
        PathGenerators = FindObjectsByType<PathGenerator>(FindObjectsSortMode.None).ToHashSet();
        StartCoroutine(RunWorldSetup());
        SceneInstance = this;

        SpawnerScript = FindObjectOfType<EnemySpawnerLogic>();
    }

    private IEnumerator RunWorldSetup()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (var GeneratorScript in PathGenerators)
        {
            GeneratorScript.HandleGeneration();
            Debug.Log(GeneratorScript.gameObject.name);
        }
        yield return new WaitForSeconds(0.025f);
        //ProgramManager.ProgramManagerInstance.SpawnWave.Invoke();

    }

}
