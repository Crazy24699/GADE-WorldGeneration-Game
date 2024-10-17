using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnerLogic : MonoBehaviour
{

    [SerializeField] private EnemyObject[] EnemyOptions;
    [SerializeField] private EnemyObject NextSpawnEnemy;

    [SerializeField] private GameObject[] Spawns;
    //[SerializeField] private List<bool> ReadyPaths;
    [SerializeField] private GameObject LastSpawn;


    //Scripts   
    private HashSet<PathGenerator> MadePaths = new HashSet<PathGenerator>();
    public WaveFunctionality[] WaveChangeFunction;


    //remove this
    public bool Spawn = true;
    private bool ProgressiveDifficulty;
    private bool WaveBeaten = false;
    private bool WaveStarted = false;


    [SerializeField] private int EnemiesToSpawn;
    [SerializeField] private int RemainingEnemies;

    [SerializeField] public int LivingEnemies;
    [SerializeField] private int MaxLivingEnemies;

    [SerializeField] private int SpawnCostAvaliable;
    [SerializeField] private int WaveSpawnCost;
    private int CurrentSpawnLocation = 0;

    [SerializeField] private int CurrentWave; 
    [SerializeField] private int MaxDownTime = 5;

    [SerializeField] private float CurrentDownTime;
    private float BaseWaveMultiplier = 1.25f;

    private void Start()
    {
        ProgramManager.ProgramManagerInstance.SpawnWave.AddListener(() => HandleWaves());
        
    }

    private void HandleWaves()
    {
        if (!Spawn) { return; }
        WaveStarted = false;
        WaveBeaten = false;
        if (MaxLivingEnemies <= 0)
        {
            MaxLivingEnemies = 21;
        }
        if (EnemiesToSpawn == 0)
        {
            EnemiesToSpawn = 30;
        }
        RemainingEnemies = EnemiesToSpawn;

        CurrentWave = 1;
        SpawnCostAvaliable = WaveSpawnCost;
        StartCoroutine(WaveLoop());
    }

    //private IEnumerator SpawnEnemies()
    //{
    //    Debug.Log("in a dream   ");
    //    yield return new WaitForSeconds(0);

    //    for (int i = 0; i < 3; i++)
    //    {
    //        if (AliveEnemies < 21)
    //        {
    //            yield return new WaitForSeconds(0.25f);
    //            CreateEnemy("Normal", CurrentSpawnNum);
    //            Debug.Log(CurrentSpawnNum);


    //            Debug.Log("i nolonger dream, only nightmares of those whove died;       " + CurrentSpawnNum);
    //            CurrentSpawnNum++;
    //        }

    //    }

    //    if (CurrentSpawnNum > 2 && AliveEnemies <= EnemySpawnCap)
    //    {
    //        CurrentSpawnNum = 0;
    //    }


    //    yield return new WaitForSeconds(1);

    //}

    public void HandleEnemyCost()
    {

        if (!EnemyOptions.ToList().Contains(NextSpawnEnemy)) { return; }

        if (NextSpawnEnemy.TypeAlive >= NextSpawnEnemy.MaxAliveType) { NextSpawnEnemy.CheckSpawning(); return; }

        if (LivingEnemies >= MaxLivingEnemies) { return; }

        int EnemyIndex = EnemyOptions.ToList().IndexOf(NextSpawnEnemy);

        //Debug.Log("Types value" + EnemyIndex);


        int SpawnCost = EnemyOptions[EnemyIndex].CalculateCost();

        int NewSpawnCost = SpawnCostAvaliable - SpawnCost;
        if (NewSpawnCost > 0)
        {
            if (CurrentSpawnLocation >= Spawns.Length)
            {
                CurrentSpawnLocation = 0;
            }

            //CanSpawn = true;
            SpawnCostAvaliable -= SpawnCost;
            //BaseEnemy EnemyScript = Instantiate(EnemyOptions[EnemyIndex].EnemyPrefabObject).GetComponent<BaseEnemy>();
            CreateEnemy(CurrentSpawnLocation, EnemyOptions[EnemyIndex].EnemyPrefabObject);
            CurrentSpawnLocation++;
            Debug.Log("spawned");
            

            EnemyOptions[EnemyIndex].EnemySpawnedCount++;
            

            //EnemyScript.Spawner = false;
            EnemyOptions[EnemyIndex].TypeAlive++;
            RemainingEnemies--;
            ReduceOtherValues(NextSpawnEnemy);

            

        }
        else
        {
            //CanSpawn = false;
        }

    }
    public void UpdateEnemies(EnemyObject ChangeObject)
    {

        EnemyObject SpecifiedObject = EnemyOptions.ToList().Find(Obj => Obj.EnemyType == ChangeObject.EnemyType);
        SpecifiedObject.TypeAlive--;
        LivingEnemies--;
        SpawnCostAvaliable += ChangeObject.CurrentSpawnCost;

    }

    public void CheckSpawnList(PathGenerator PathGenRef)
    {
        MadePaths.Add(PathGenRef);
        //Debug.Log(MadePaths.Count);
        if (MadePaths.Count >= 3)
        {
            ProgramManager.ProgramManagerInstance.SpawnWave.Invoke();
        }
    }

    private void Update()
    {

        if (RemainingEnemies <= 0)
        {
            WaveStarted = false;
        }
        if (!WaveStarted && LivingEnemies <= 0 && !WaveBeaten)
        {
            WaveBeaten = true;
            CurrentDownTime = MaxDownTime;
        }

        WaveDownTime();
        if (Input.GetKeyDown(KeyCode.R))
        {
            LivingEnemies--;
        }
    }

    private void ReduceOtherValues(EnemyObject LowestEnemy)
    {
        EnemyObject[] OtherEnemies = EnemyOptions.ToList().Where(C => C.EnemyType != LowestEnemy.EnemyType).ToArray();
        foreach (var Enemy in OtherEnemies)
        {
            if (!Enemy.CanSpawn) { break; }
            //Debug.Log(Enemy.EnemyType);
            Enemy.ReduceCost();
        }
    }

    private EnemyObject FindLowestCost()
    {
        int LowestCostFound = 100;
        EnemyObject LowestCost = null;

        foreach (var Enemy in EnemyOptions)
        {
            if (!Enemy.CanSpawn || !Enemy.CheckSpawning()) { break; }

            int EnemyCost = Enemy.CalculateCost();
            //Debug.Log(EnemyCost + "       ");
            if (EnemyCost < LowestCostFound)
            {
                LowestCost = Enemy;
                LowestCostFound = EnemyCost;
            }

        }
        if (LowestCost == null)
        {
            int EnemyCounter = 0;
            foreach (var Enemy in EnemyOptions)
            {
                if (Enemy.CanSpawn)
                {
                    LowestCost = Enemy;
                }
                EnemyCounter++;
            }
        }

        return LowestCost;
    }

    private IEnumerator WaveLoop()
    {
        if (WaveStarted) { yield break; }

        int EnemyIndex = 0;

        WaveStarted = true;
        while (WaveStarted)
        {
            yield return new WaitForSeconds(0.5f);
            //Debug.Log("How long ");

            EnemyOptions[EnemyIndex].CheckSpawning();

            NextSpawnEnemy = FindLowestCost();
            HandleEnemyCost();

            EnemyIndex++;
            if (EnemyIndex >= EnemyOptions.Length)
            {
                EnemyIndex = 0;
            }
        }
    }

    private void WaveDownTime()
    {
        if (!WaveBeaten) return;
        CurrentDownTime -= Time.deltaTime;
        if (CurrentDownTime < 0)
        {
            StartWave();
        }
    }

    private void StartWave()
    {
        CurrentWave++;
        if (WaveChangeFunction.Any(Wave => Wave.WaveNumber == CurrentWave))
        {
            SpawnCostAvaliable = Mathf.RoundToInt(WaveSpawnCost * (CurrentWave * 0.75f));
            //SpawnCostAvaliable = WaveSpawnCost * (Mathf.RoundToInt((CurrentWave * BaseWaveMultiplier) * WaveChangeFunction[CurrentWave].WaveCostMultiplier));
        }
        else
        {
            Debug.Log("hell");
            SpawnCostAvaliable = Mathf.RoundToInt(WaveSpawnCost * (CurrentWave * 0.75f));
        }
        

        if (MaxLivingEnemies <= 0)
        {
            MaxLivingEnemies = 21;
        }
        if (EnemiesToSpawn == 0)
        {
            EnemiesToSpawn = 30;
        }
        RemainingEnemies = EnemiesToSpawn;
        WaveBeaten = false;

        StartCoroutine(WaveLoop());
    }

    private void CreateEnemy(int SpawnLocation, GameObject EnemyPrefab)
    {
        Debug.Log("monseter     " + SpawnLocation);
        GameObject EnemyCreated = null;

        EnemyCreated = Instantiate(EnemyPrefab, Spawns[SpawnLocation].transform.position + new Vector3(2.5f, 1.5f, 0), Quaternion.identity);
        EnemyCreated.GetComponent<BaseEnemy>().Startup();
        EnemyObject EnemyScript = EnemyCreated.GetComponent<BaseEnemy>().EnemyType;
        int Index = Spawns[SpawnLocation].GetComponent<PathGenerator>().ControlPointTransforms.Count();

        EnemyCreated.GetComponent<BaseEnemy>().FinalTarget = Spawns[SpawnLocation].GetComponent<PathGenerator>().ControlPointTransforms[Index - 1].gameObject;
        EnemyCreated.gameObject.name = EnemyCreated.name + "    Spawned Enemy" + LivingEnemies + " " + EnemyScript.EnemyType + "   " + EnemyScript.TypeAlive + "" + Random.Range(0, 800);
        //ProgramManager.ProgramManagerInstance.EnemyCount.Add(EnemyCreated);
        EnemyCreated.GetComponent<BaseEnemy>().ParentSpawner = this;

        int EnemyIndex = EnemyOptions.ToList().IndexOf(NextSpawnEnemy);
        EnemyCreated.GetComponent<BaseEnemy>().PopulateValues(EnemyOptions[EnemyIndex].SpawnedEnemyObject());

        LivingEnemies++;
        LastSpawn = Spawns[SpawnLocation];
    }

}

#region Waves and enemies
[System.Serializable]
public class EnemyObject
{
    public GameObject EnemyPrefabObject;
    [SerializeField] private int BaseCost;
    [SerializeField] public int CurrentSpawnCost;

    public int TypeAlive;
    public int MaxAliveType;
    public int EnemySpawnedCount;
    public int TestValue;

    public bool CanSpawn = false;
    public bool Alive = false;

    public string EnemyType;

    public int CalculateCost()
    {
        //if (Alive) { return CurrentSpawnCost; }
        int MaxCostMultiplier = 0;
        if (TypeAlive >= MaxAliveType)
        {
            MaxCostMultiplier = (BaseCost * TypeAlive) * 2;
        }
        CurrentSpawnCost = BaseCost * (TypeAlive + 1) + MaxCostMultiplier;
        Debug.Log("Calcualte code");

        return CurrentSpawnCost;

    }

    public int ReduceCost()
    {
        return CurrentSpawnCost -= BaseCost;
    }

    public EnemyObject SpawnedEnemyObject()
    {
        EnemyObject EnemyObject = new EnemyObject();

        EnemyObject.BaseCost = BaseCost;
        EnemyObject.EnemyPrefabObject = EnemyPrefabObject;
        EnemyObject.CurrentSpawnCost = CurrentSpawnCost;
        EnemyObject.TypeAlive = TypeAlive;
        EnemyObject.MaxAliveType = MaxAliveType;
        EnemyObject.EnemyType = EnemyType;
        EnemyObject.EnemySpawnedCount = EnemySpawnedCount;

        return EnemyObject;
    }

    public bool CheckSpawning()
    {
        if (TypeAlive >= MaxAliveType)
        {
            CanSpawn = false;
            return false;
        }
        CanSpawn = true;
        return true;
    }

}


[System.Serializable]
public class WaveFunctionality
{

    [SerializeField] public int WaveNumber;
    [SerializeField] public float WaveCostMultiplier;

    [SerializeField] private GameObject EnemyObject;
    [SerializeField] private float PrepTimeChange;
    [SerializeField] public readonly bool ChangeCostMultiplier;
    
    public float NewCostMuliplier()
    {
        float NewMultiplier = 0.0f;

        NewMultiplier = WaveNumber + 5;
        NewMultiplier = (WaveNumber / (WaveNumber - 0.75f));



        return 0;
    }

}

#endregion

