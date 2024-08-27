using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MapChunk
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
    private HeightMapValues HeightMapValuesRef;
    private MeshGenerator MeshGeneratorScript;

    private LODInfo[] DetailLevels;
    public MeshLOD[] LODMeshes;

    private int LODColliderIndex;
    private int LastLODIndex = -1;

    private float MaxViewDistamce;

    public bool MeshRequested;

    public MapChunk(Vector2 Coordinate, HeightMapGenerator HeightMapGenerator, MeshGenerator MeshGenerator, LODInfo[] DetailLevelsRef, int CollisionLODIndex, Transform Parent, Material Mat)
    {
        ChunkCoordinate = Coordinate;
        DetailLevels = DetailLevelsRef;
        LODColliderIndex = CollisionLODIndex;

        HeightMapGeneratorScript = HeightMapGenerator;
        MeshGeneratorScript = MeshGenerator;

        Vector2 Position = Coordinate;
        ChunkBounds = new Bounds(ChunkCoordinate, Vector2.one * MeshGeneratorScript.MeshScale);

        MeshGameObject = new GameObject("WorldChunk");
        MeshFilterRef = MeshGameObject.AddComponent<MeshFilter>();
        MeshRendererRef = MeshGameObject.AddComponent<MeshRenderer>();
        MeshColliderRef = MeshGameObject.AddComponent<MeshCollider>();
        MeshRendererRef.material = Mat;

        MeshGameObject.transform.position = new Vector3(ChunkCoordinate.x, 0, ChunkCoordinate.y);
        MeshGameObject.transform.parent = Parent;

        LODMeshes = new MeshLOD[DetailLevels.Length];
        for (int i = 0; i < LODMeshes.Length; i++)
        {
            LODMeshes[i] = new MeshLOD(DetailLevels[i].LODLevel);
            //LODMeshes[i].MeshUpdateEvent.AddListener(UpdateChunk);
            //if (i == CollisionLODIndex)
            //{

            //}
        }

    }

    public void LoadChunk()
    {
        int VertsPerLineRef=MeshGeneratorScript.VerticesPerLine;
        Debug.Log(VertsPerLineRef);
        HeightMapGenerator HeightMapGeneration = GameObject.FindObjectOfType<HeightMapGenerator>();
        HeightMapGeneration.GenerateHeightMap(VertsPerLineRef, VertsPerLineRef, CenterCoordinate);
        Debug.Log(HeightMapGeneration != null);

        HeightMapValuesRecived(HeightMapGeneration.GenerateHeightMap(VertsPerLineRef, VertsPerLineRef, CenterCoordinate));
    }

    private void HeightMapValuesRecived(object HeightMapObject)
    {
        HeightMapValuesRef = (HeightMapValues)HeightMapObject;
        MeshRequested = true;

        UpdateChunk();
    }

    public void UpdateChunk()
    {
        int LODIndex=0;
        for (int i = 0; i < LODMeshes.Length; i++)
        {
            Debug.Log("unleaded now");
            MeshLOD MeshLODRef = LODMeshes[i];
            if (LODMeshes[i].HasMesh)
            {
                LastLODIndex = LODIndex;
                MeshFilterRef.mesh = MeshLODRef.MeshRef;

            }
            else if (!LODMeshes[i].MeshRequested)
            {
                Debug.Log("unleaded nothing to do");
                MeshLODRef.RequestMesh(HeightMapValuesRef, MeshGeneratorScript);

            }
        }
    }

}

[System.Serializable]
public class MeshLOD
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

    void OnMeshDataReceived(object meshDataObject)
    {
        MeshRef = new Mesh();
        MeshRef = ((MeshData)meshDataObject).CreateMesh();
        HasMesh = true;
        Debug.Log(((MeshData)meshDataObject).vertices.Length);
        
    }

    public void RequestMesh(HeightMapValues HeightMapScript, MeshGenerator MeshGenScript)
    {
        MeshRequested = true;

        //MeshGenScript.GenerateTerrainMesh(HeightMapScript.HeightValues, LODValue);
        OnMeshDataReceived(MeshGenScript.GenerateTerrainMesh(HeightMapScript.HeightValues, LODValue));

    }

}
