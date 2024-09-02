using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Chunk : MonoBehaviour 
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
    public ChunkInfo[] NeighbouringChunks;

    public MapGenerator MapGeneratorScript;
    private TextureGenerator TextureGenScript;
    public MeshGenerationData GenerationData;

    [SerializeField]private ChunkInfo ChunkInfoRef;

    public MapDisplayStruct MapDataVar;
    public bool MapDataReceived;
    public int PreviousLodIndex = -1;

    public Chunk()
    {
       
    }

    public void ApplyMeshData(MeshGenerationData MeshData)
    {
        MeshFilterVar = GetComponent<MeshFilter>();
        Debug.Log(MeshData.BorderVertices[5]);
        MeshFilterVar.mesh=MeshData.CreateMesh();
        Debug.Log(MeshData.BorderVertices[5]);
        ChunkInfo Info = GetComponent<ChunkInfo>();

        //Debug.Log(MapGenerator.MeshDataInfoThread.FirstOrDefault(Position));



        Info.BorderVertices = MeshData.BorderVertices;
    }

    public void SetValues(Vector2 CoordVar, int SizeVar, LODInfoClass[] DetailLevelsVar, Transform ParentVar, Material MaterialVar)
    {
        Debug.Log("Live");

        TextureGenScript = new TextureGenerator();

        DetailLevels = DetailLevelsVar;
        MapGeneratorScript = GameObject.FindObjectOfType<MapGenerator>();
        //GenerationData=MapGeneratorScript.mesh
        Position = CoordVar * SizeVar;
        BoundsVar = new Bounds(Position, Vector2.one * SizeVar);
        Vector3 PositionV3 = new Vector3(Position.x, 0, Position.y);

        MeshObject = this.gameObject;
        MeshObject.AddComponent<ChunkInfo>();

        //ChunkInfoScript.BorderVertices = MeshData.BorderVertices;

        ChunkInfoRef = GetComponent<ChunkInfo>();

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

        if(GenerationData!=null)
        {
            Debug.Log("sick cowards");
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
        if (GenerationData != null)
        {
            Debug.Log("his swons y firend");
        }
        
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

        //foreach (var Vertex in VertexList)
        //{
        //    if (ChunkInfoRef.BorderVertices.Contains(Vertex))
        //    {


        //    }
        //}

        //for (int i = 0; i < ChunkInfoRef.AllVertices.Length; i++)
        //{
        //    //if (i< VertexList.l && VertexList[i] == ChunkInfoRef.AllVertices[i])
        //    //{
        //    //    Debug.Log("Das nothing");
        //    //    ChunkInfoRef.AllVertices[i].y = 0;
        //    //    ChunkInfoRef.AllVertices[i].z = 0;
        //    //}
        //    //VertexList[i].y = 0;
        //}
        MeshFilterVar.sharedMesh.vertices = ChunkInfoRef.AllVertices;
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
                    MeshFilterVar.mesh = LODMeshVar.MeshRef;
                }

                else if (!LODMeshVar.HasRequestedMesh)
                {
                    LODMeshVar.RequestMesh(MapDataVar);
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
                    MeshColliderVar.sharedMesh = CollisionLodMesh.MeshRef;
                }
                else if (!CollisionLodMesh.HasRequestedMesh)
                {
                    //CollisionLodMesh.RequestMesh(MapDataVar);
                }
            }

            MapGenerator.AllVisableChunks.Add(this);
        }
        if (GenerationData != null)
        {
            Debug.Log("hell make everyone bleed");
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

public class ChunkNeighbour
{

    HashSet<Vector3> Border;

}