using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnerLogic : MonoBehaviour
{

    [SerializeField] private GameObject[] EnemyOptions;
    [SerializeField] private GameObject[] Spawns;
    //[SerializeField] private List<bool> ReadyPaths;
    private HashSet<PathGenerator> MadePaths = new HashSet<PathGenerator>();
    private GameObject LastSpawn;

    //remove this
    public bool Spawn = true;

    private int CurrentSpawner = 0;

    private void Start()
    {
        ProgramManager.ProgramManagerInstance.SpawnWave.AddListener(() => InvokeRepeating(nameof(HandleWaves), 0.0f, 1.25f));
    }

    private void HandleWaves()
    {
        if(!Spawn) { return; }
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 3; i++)
        {
            if (ProgramManager.ProgramManagerInstance.EnemyCount.Count < 21)
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
        //Debug.Log(MadePaths.Count);
        if(MadePaths.Count >= 3)
        {
            ProgramManager.ProgramManagerInstance.SpawnWave.Invoke();
        }
    }

    private void CreateEnemy(string EnemyType, int SpawnLocation)
    {
        Debug.Log("monseter");
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
        int Index = Spawns[SpawnLocation].GetComponent<PathGenerator>().ControlPointTransforms.Count();

        EnemyCreated.GetComponent<BasicEnemy>().FinalTarget = Spawns[SpawnLocation].GetComponent<PathGenerator>().ControlPointTransforms[Index-1].gameObject;
        EnemyCreated.gameObject.name = "Normal Enemy"+ProgramManager.ProgramManagerInstance.EnemyCount.Count;
        ProgramManager.ProgramManagerInstance.EnemyCount.Add(EnemyCreated);
    }

}
