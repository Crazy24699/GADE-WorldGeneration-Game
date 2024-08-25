using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int ColliderLODIndex;

    public LODInfo[] LevelDetails;

    

}

[System.Serializable]
public struct LODInfo
{
    [Range(0,6)]
    public int LODLevel;
    public float VisibleDistanceLimit;

    public float SqurVisibleDistanceLimit(float VisableDistanceSqrt)
    {
        return VisableDistanceSqrt;
    }

}
