using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static MapGenerator;

public class Chunk 
{
    public const float Scale = 2.5f;
    public GameObject MeshObject;
    public Vector2 Position;
    public Bounds BoundsVar;

    public MeshRenderer MeshRendererVar;
    public MeshFilter MeshFilterVar;
    public MeshCollider MeshColliderVar;

    public LODInfoClass[] DetailLevels;
    public LODMeshClass[] LodMeshes;
    public LODMeshClass CollisionLodMesh;

    public MapGenerator MapGeneratorScript;
    private TextureGenerator TextureGenScript;

    public MapDisplayStruct MapDataVar;
    public bool MapDataReceived;
    public int PreviousLodIndex = -1;

    public Chunk(Vector2 CoordVar, int SizeVar, LODInfoClass[] DetailLevelsVar, Transform ParentVar, Material MaterialVar)
    {
        Debug.Log("Live");

        TextureGenScript = new TextureGenerator();

        DetailLevels = DetailLevelsVar;
        MapGeneratorScript = GameObject.FindObjectOfType<MapGenerator>();
        Position = CoordVar * SizeVar;
        BoundsVar = new Bounds(Position, Vector2.one * SizeVar);
        Vector3 PositionV3 = new Vector3(Position.x, 0, Position.y);

        MeshObject = new GameObject("Terrain Chunk");
        MeshRendererVar = MeshObject.AddComponent<MeshRenderer>();
        MeshFilterVar = MeshObject.AddComponent<MeshFilter>();
        MeshColliderVar = MeshObject.AddComponent<MeshCollider>();
        MeshRendererVar.material = MaterialVar;

        MeshObject.transform.position = PositionV3 * Scale;
        MeshObject.transform.parent = ParentVar;
        MeshObject.transform.localScale = Vector3.one * Scale;
        SetVisible(false);

        LodMeshes = new LODMeshClass[DetailLevels.Length];
        for (int i = 0; i < DetailLevels.Length; i++)
        {
            LodMeshes[i] = new LODMeshClass(DetailLevels[i].Lod, UpdateTerrainChunk, MapGeneratorScript);
            if (DetailLevels[i].UseForCollider)
            {
                CollisionLodMesh = LodMeshes[i];
            }
        }

        MapGeneratorScript.RequestMapData(Position, OnMapDataReceived);
    }

    void OnMapDataReceived(MapDisplayStruct MapDispalyDataRef)
    {
        MapDataVar = MapDispalyDataRef;
        MapDataReceived = true;

        Texture2D TextureVar = TextureGenerator.ColourMapTextureGen(MapDataVar.ColourMapValues, MapGenerator.MapChunkSize, MapGenerator.MapChunkSize);
        MeshRendererVar.material.mainTexture = TextureVar;

        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk()
    {
        if (MapDataReceived)
        {
            int LodIndexVar = 0;

            for (int i = 0; i < DetailLevels.Length - 1; i++)
            {
                LodIndexVar = i + 1;
            }

            if (LodIndexVar != PreviousLodIndex)
            {
                LODMeshClass LodMeshVar = LodMeshes[LodIndexVar];
                if (LodMeshVar.HasMesh)
                {
                    PreviousLodIndex = LodIndexVar;
                    MeshFilterVar.mesh = LodMeshVar.MeshRef;
                }
                else if (!LodMeshVar.HasRequestedMesh)
                {
                    LodMeshVar.RequestMesh(MapDataVar);
                }
            }

            if (LodIndexVar == 0)
            {
                if (CollisionLodMesh.HasMesh)
                {
                    MeshColliderVar.sharedMesh = CollisionLodMesh.MeshRef;
                }
                else if (!CollisionLodMesh.HasRequestedMesh)
                {
                    CollisionLodMesh.RequestMesh(MapDataVar);
                }
            }

            MapGenerator.AllVisableChunks.Add(this);
        }

        SetVisible(true);
    }

    public void SetVisible(bool VisibleVar)
    {
        MeshObject.SetActive(VisibleVar);
    }

    public bool IsVisible()
    {
        return MeshObject.activeSelf;
    }
}
