using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnerLogic : MonoBehaviour
{

    [SerializeField] private GameObject[] EnemyOptions;
    [SerializeField] private GameObject[] Spawns;

    private int CurrentSpawner = 0;

    private void Start()
    {
        ProgramManager.ProgramManagerInstance.SpawnWave.AddListener(() => StartCoroutine(SpawnEnemies()));
    }

    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < 3; i++)
        {
            if (ProgramManager.ProgramManagerInstance.EnemyCount.Count < 12)
            {
                CreateEnemy("Normal", i);
            }
            yield return new WaitForSeconds(1);

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
        EnemyCreated = Instantiate(ChosenEnemy, Spawns[SpawnLocation].transform.position, Quaternion.identity);
        EnemyCreated.GetComponent<BaseEnemy>().Startup();
        ProgramManager.ProgramManagerInstance.EnemyCount.Add(EnemyCreated);
    }

}
