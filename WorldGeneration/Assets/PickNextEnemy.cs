using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickNextEnemy : MonoBehaviour
{
    [SerializeField] public SpawnEnemyObject[] EnemyOptions;
    private SpawnEnemyObject NextSpawnEnemy;
    

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