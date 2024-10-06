using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickNextEnemy : MonoBehaviour
{
    [SerializeField] public SpawnEnemyObject[] EnemyOptions;
    private SpawnEnemyObject NextSpawnEnemy;
    
    //this is so i have soemthing to commit.
    //The work was done on tartarus. The whole wave system seems to work perfectly
    public void HandleEnemyCost()
    {

    }

}

[System.Serializable]
public class SpawnEnemyObject
{
    [SerializeField] private int BaseCost;
    [SerializeField] private GameObject EnemyObject;
    [SerializeField] private int CurrentSpawnCost;

}