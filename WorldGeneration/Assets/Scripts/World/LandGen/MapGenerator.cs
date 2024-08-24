using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int ColliderLODIndex;

    public LODInfo[] LevelDetails;

    public MeshSettings MeshSettingsRef;
    public HeightMapSettings HightSettingsRef;

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
