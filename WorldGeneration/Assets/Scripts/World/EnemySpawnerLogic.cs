using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnerLogic : MonoBehaviour
{

    [SerializeField] private GameObject[] EnemyOption;
    [SerializeField] private GameObject[] Spawns;
    //[SerializeField] private List<bool> ReadyPaths;
    [SerializeField]private GameObject LastSpawn;


    //Scripts   
    private HashSet<PathGenerator> MadePaths = new HashSet<PathGenerator>();
    public SpawnEnemyObject[] EnemyOptions;
    public WaveFunctionality[] WaveChangeFunction;


    //remove this
    public bool Spawn = true;
    private bool ProgressiveDifficulty;
    private bool WaveBeaten;
    

    [SerializeField]private int AliveEnemies = 0;
    public int CurrentSpawnNum;
    private int SpawnCostAvaliable;
    private int CurrentWave;

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
        Debug.Log("in a dream   ");
        yield return new WaitForSeconds(0);

        for (int i = 0; i < 3; i++)
        {
            if (AliveEnemies < 21)
            {
                yield return new WaitForSeconds(0.25f);
                CreateEnemy("Normal", CurrentSpawnNum);
                Debug.Log(CurrentSpawnNum);


                Debug.Log("i nolonger dream, only nightmares of those whove died;       " + CurrentSpawnNum);
                CurrentSpawnNum++;
            }

        }

        if (CurrentSpawnNum > 2 && AliveEnemies <= 21)
        {
            CurrentSpawnNum = 0;
        }


        yield return new WaitForSeconds(1);

    }

    private void CheckWaveChange()
    {
        if (CurrentWave == 5) 
        {

        }
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            AliveEnemies--;
        }
    }

    private void CreateEnemy(string EnemyType, int SpawnLocation)
    {
        Debug.Log("monseter     "+SpawnLocation);
        GameObject EnemyCreated = null;
        GameObject ChosenEnemy = null;
        switch (EnemyType)
        {
            default:
            case "Normal":
                ChosenEnemy = EnemyOption[0].gameObject;
                break;
        }
        EnemyCreated = Instantiate(ChosenEnemy, Spawns[SpawnLocation].transform.position + new Vector3(2.5f, 1.5f, 0), Quaternion.identity);
        EnemyCreated.GetComponent<BaseEnemy>().Startup();
        int Index = Spawns[SpawnLocation].GetComponent<PathGenerator>().ControlPointTransforms.Count();

        EnemyCreated.GetComponent<BasicEnemy>().FinalTarget = Spawns[SpawnLocation].GetComponent<PathGenerator>().ControlPointTransforms[Index - 1].gameObject;
        EnemyCreated.gameObject.name = "Normal Enemy" + ProgramManager.ProgramManagerInstance.EnemyCount.Count;
        ProgramManager.ProgramManagerInstance.EnemyCount.Add(EnemyCreated);
        AliveEnemies++;
        LastSpawn = Spawns[SpawnLocation];
    }

}


[System.Serializable]
public class SpawnEnemyObject
{
    [SerializeField] private int BaseCost;
    [SerializeField] private GameObject EnemyObject;
    [SerializeField] private int Living;
}

[System.Serializable]
public class WaveFunctionality
{

    [SerializeField] private int WaveNumber;
    [SerializeField] private int WaveCostMultiplier;

    [SerializeField] private GameObject EnemyObject;
    [SerializeField] private float PrepTimeChange;


}