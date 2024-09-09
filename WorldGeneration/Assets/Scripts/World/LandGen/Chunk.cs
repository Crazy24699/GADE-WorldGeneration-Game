using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Chunk : MonoBehaviour 
{
    public const float Scale = 1;
    public GameObject MeshObject;
    public Vector2 Position;
    public Bounds ChunkBounds;

    public MeshRenderer MeshRendererRef;
    public MeshFilter MeshFilterRef;
    public MeshCollider MeshColliderRef;

    public LODInfoClass[] DetailLevels;
    public LODMeshClass[] LodMeshes;
    public LODMeshClass CollisionLodMesh;
    public ChunkInfo[] NeighbouringChunks;

    public MapGenerator MapGeneratorScript;
    private TextureGenerator TextureGenScript;
    public MeshGenerationData GenerationData;

    [SerializeField]private ChunkInfo ChunkInfoRef;

    public MapDisplayStruct MapDataScript;
    public bool MapDataReceived;
    public int PreviousLodIndex = -1;

    public Chunk()
    {
       
    }

    public void ApplyMeshData(MeshGenerationData MeshData)
    {
        MeshFilterRef = GetComponent<MeshFilter>();
        Debug.Log(MeshData.BorderVertices[5]);
        MeshFilterRef.mesh=MeshData.CreateMesh();
        Debug.Log(MeshData.BorderVertices[5]);
        ChunkInfo Info = GetComponent<ChunkInfo>();

        //Debug.Log(MapGenerator.MeshDataInfoThread.FirstOrDefault(Position));



        Info.BorderVertices = MeshData.BorderVertices;
    }

    public void SetValues(Vector2 ChunkCord, int ChunkSize, LODInfoClass[] DetailLevels, Transform ParentObject, Material MaterialRef)
    {
        //Debug.Log("Live");

        TextureGenScript = new TextureGenerator();

        this.DetailLevels = DetailLevels;
        MapGeneratorScript = GameObject.FindObjectOfType<MapGenerator>();
        //GenerationData=MapGeneratorScript.mesh

        Position = ChunkCord * ChunkSize;
        ChunkBounds = new Bounds(Position, Vector2.one * ChunkSize);
        Vector3 PositionV3 = new Vector3(Position.x, 0, Position.y);

        MeshObject = this.gameObject;
        MeshObject.AddComponent<ChunkInfo>();

        //ChunkInfoScript.BorderVertices = MeshData.BorderVertices;

        ChunkInfoRef = GetComponent<ChunkInfo>();

        MeshRendererRef = MeshObject.AddComponent<MeshRenderer>();
        MeshFilterRef = MeshObject.AddComponent<MeshFilter>();
        MeshColliderRef = MeshObject.AddComponent<MeshCollider>();

        MeshRendererRef.material = MaterialRef;

        MeshObject.transform.position = PositionV3 * Scale;
        MeshObject.transform.parent = ParentObject;
        MeshObject.transform.localScale = Vector3.one * Scale;
        SetVisible(false);

        LodMeshes = new LODMeshClass[this.DetailLevels.Length];
        for (int i = 0; i < this.DetailLevels.Length; i++)
        {
            LodMeshes[i] = new LODMeshClass(this.DetailLevels[i].Lod, UpdateTerrainChunk, MapGeneratorScript);
            if (this.DetailLevels[i].UseForCollider)
            {
                CollisionLodMesh = LodMeshes[i];
            }
        }

        if(GenerationData!=null)
        {
            Debug.Log("sick cowards");
        }

        MapGeneratorScript.RequestMapData(Position, OnMapDataReceived);
        
    }

    void OnMapDataReceived(MapDisplayStruct MapDispalyDataRef)
    {
        MapDataScript = MapDispalyDataRef;
        MapDataReceived = true;

        //Texture2D TextureVar = TextureGenerator.ColourMapTextureGen(MapDataScript.ColourMapValues, MapGenerator.MapChunkSize, MapGenerator.MapChunkSize);
        //MeshRendererRef.material.mainTexture = TextureVar;

        UpdateTerrainChunk();
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            EditMesh();
        }
    }

    public void EditMesh()
    {
        List<Vector3> VertexList = ChunkInfoRef.AllVertices.ToList();
        List<Vector3> BorderVertices = ChunkInfoRef.BorderVertices.ToList();

        for (int i = 0; i < VertexList.Count; i++)
        {
            if (BorderVertices.Contains(VertexList[i]))
            {
                VertexList[i] = new Vector3(VertexList[i].x, 0, VertexList[i].z);
            }
        }

        ChunkInfoRef.AllVertices = VertexList.ToArray();

        MeshFilterRef.sharedMesh.vertices = ChunkInfoRef.AllVertices;
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
                LODMeshClass LODMeshVar = LodMeshes[LodIndexVar];
                if (LODMeshVar.HasMesh)
                {
                    PreviousLodIndex = LodIndexVar;
                    MeshFilterRef.mesh = LODMeshVar.MeshRef;
                }

                else if (!LODMeshVar.HasRequestedMesh)
                {
                    LODMeshVar.RequestMesh(MapDataScript);
                    LODMeshVar.MadeChunk = this.gameObject;
                    
                }
                if (LODMeshVar.MadeChunk != null) 
                {
                    //LODMeshVar.MadeChunk.GetComponent<MeshInfo>().MeshId = LodIndexVar;
                }
            }

            if (LodIndexVar == 0)
            {
                if (CollisionLodMesh.HasMesh)
                {
                    MeshColliderRef.sharedMesh = CollisionLodMesh.MeshRef;
                }
                else if (!CollisionLodMesh.HasRequestedMesh)
                {
                    //CollisionLodMesh.RequestMesh(MapDataVar);
                }
            }

            //MapGenerator.AllVisableChunks.Add(this);
        }
        if (GenerationData != null)
        {
            //Debug.Log("hell make everyone bleed");
        }
        this.GetComponent<MeshCollider>().sharedMesh = MeshFilterRef.sharedMesh;

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

