using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapChunk : MonoBehaviour
{
    private const float ColliderGeneration = 5;

    public Vector2 ChunkCoordinate;
    private Vector2 CenterCoordinate;
    private Bounds ChunkBounds;

    private GameObject MeshGameObject;
    private MeshRenderer MeshRendererRef;
    private MeshFilter MeshFilterRef;
    private MeshCollider MeshColliderRef;
    
    private HeightMapGenerator HeightMapGeneratorScript;
    private MeshGenerator MeshGeneratorScript;

    private LODInfo[] DetailLevels;
    private MeshLOD[] LODMeshes;

    private int LODColliderIndex;
    private int LastLODIndex = -1;

    private float MaxViewDistamce;
    
    public MapChunk(Vector2 Coordinate, HeightMapGenerator HeightMapGenerator, MeshGenerator MeshGenerator, LODInfo[] DetailLevelsRef, int CollisionLODIndex, Transform Parent, Material Mat)
    {
        ChunkCoordinate = Coordinate;
        DetailLevels = DetailLevelsRef;
        LODColliderIndex = CollisionLODIndex;

        HeightMapGeneratorScript = HeightMapGenerator;
        MeshGeneratorScript = MeshGenerator;

        Vector2 Position = Coordinate * MeshGeneratorScript.MeshWorldScale / MeshGeneratorScript.MeshScale;

    }

}

class MeshLOD
{
    public Mesh MeshRef;
    public bool MeshRequested;
    public bool HasMesh;

    private int LODValue;

    public UnityEvent MeshUpdateEvent;

    public MeshLOD (int LODValueRef)
    {
        LODValue = LODValueRef;
    }

    private void MeshDataUploaded(object MeshDataObject)
    {
        MeshRef = ((Mesh)MeshDataObject);
    }

}
