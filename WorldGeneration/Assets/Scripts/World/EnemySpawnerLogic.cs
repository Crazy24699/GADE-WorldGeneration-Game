using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnerLogic : MonoBehaviour
{

    [SerializeField] private GameObject[] EnemyOptions;
    [SerializeField] private GameObject[] Spawns;
    //[SerializeField] private List<bool> ReadyPaths;
    private HashSet<PathGenerator> MadePaths = new HashSet<PathGenerator>();

    private int CurrentSpawner = 0;

    private void Start()
    {
        ProgramManager.ProgramManagerInstance.SpawnWave.AddListener(() => StartCoroutine(SpawnEnemies()));
    }

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 3; i++)
        {
            if (ProgramManager.ProgramManagerInstance.EnemyCount.Count < 12)
            {
                CreateEnemy("Normal", i);
                Debug.Log("i nolonger dream, only nightmares of those whove died;");
            }


        }
        yield return new WaitForSeconds(1);
        
    }

    public void HandleSpawningState(bool SpawnEnemies)
    {
        switch (SpawnEnemies)
        {
            case true:
                break;

            case false:
                break;

        }
    }

    public void CheckSpawnList(PathGenerator PathGenRef)
    {
        MadePaths.Add(PathGenRef);
        Debug.Log(MadePaths.Count);
        if(MadePaths.Count >= 3)
        {
            ProgramManager.ProgramManagerInstance.SpawnWave.Invoke();
        }
    }

    private void CreateEnemy(string EnemyType, int SpawnLocation)
    {
        GameObject EnemyCreated = null;
        GameObject ChosenEnemy = null;
        switch (EnemyType)
        {
            default:
            case "Normal":
                ChosenEnemy = EnemyOptions[0].gameObject;
                break;
        }
        EnemyCreated = Instantiate(ChosenEnemy, Spawns[SpawnLocation].transform.position+new Vector3(2.5f,1.5f,0), Quaternion.identity);
        EnemyCreated.GetComponent<BaseEnemy>().Startup();
        EnemyCreated.gameObject.name = "Normal Enemy"+ProgramManager.ProgramManagerInstance.EnemyCount.Count;
        ProgramManager.ProgramManagerInstance.EnemyCount.Add(EnemyCreated);
    }

}
