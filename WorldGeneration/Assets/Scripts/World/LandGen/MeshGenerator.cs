using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    //Ints
    private const int ThreadSize = 4;
    public int PointsPerAxis;

    [Space(2)]
    [Header("Scripts"), Space(2)]
    [SerializeField] private LandDensityGenerator DensityGeneratorScript;


    //Bools
    [SerializeField] private bool IsMapSizeFixed;
    public bool GenerateColliders;
    
    //Vectors
    public Vector3Int ChunkNumber;


    //Gameobjects
    public GameObject MapParent;

    //Floats
    public float ViewDistance = 20;
    
        //Meaning: The Isometric level of the mesh
    public float ISOLevel;
    public float BoundarySize;

    //Copute
    public ComputeShader Shader;


    //Buffers
    private ComputeBuffer TriangleBuffer;
    private ComputeBuffer VertexBuffer;
    private ComputeBuffer TriangleCountBuffer;


    //Scripts
    private List<Chunk> AllChunks = new List<Chunk>();
    private Dictionary<Vector3Int, Chunk> CreatedChunks = new Dictionary<Vector3Int, Chunk>();
    public Material ChunkMaterial;

    //Need to use queues for the first in and first out nature of them 
    //in regards to marching cubes mesh generation 
    private Queue<Chunk> UseableChunks = new Queue<Chunk>();

    public void UpdateMesh()
    {

    }

    public void CreateMap()
    {
        MapParent = MapParent == null ? GameObject.Find("MapParent") : new GameObject("MapParent");
    }

    private void CreateChunkBuffers()
    {
        
        int ChunkVertices = PointsPerAxis ^ 3;
        int VoxelsPerAxis = PointsPerAxis - 1;
        int VoxelCount = VoxelsPerAxis ^ 3;
        int MaxTriangleCount = VoxelCount * 5;

        if (!Application.isPlaying || (VertexBuffer == null || ChunkVertices != VertexBuffer.count))
        {
            if(Application.isPlaying)
            {
                ReleaseBuffers();
            }
            TriangleBuffer = new ComputeBuffer(MaxTriangleCount, sizeof(float) * 3 * 3);
            VertexBuffer = new ComputeBuffer(ChunkVertices, sizeof(float) * 4);
            TriangleCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        }

    }

    private void ReleaseBuffers()
    {
        if(TriangleBuffer != null)
        {
            TriangleBuffer.Release();
            VertexBuffer.Release();
            TriangleCountBuffer.Release();
        }
    }

    private Chunk CreateChunk(Vector3Int Coordinate)
    {
        GameObject ChunkObject = new GameObject($"({Coordinate.x}, {Coordinate.y}, {Coordinate.z})");
        ChunkObject.transform.parent = MapParent.transform;
        Chunk ChunkCreated = ChunkObject.AddComponent<Chunk>();
        ChunkCreated.Coordinate = Coordinate;

        return ChunkCreated;
    }

    private void UpdateChunk()
    {
        TriangleStruct[] Triangles = new TriangleStruct[1];
    }

    private void InitializeChunk()
    {
        CreateMap();
        AllChunks = new List<Chunk>();

        List<Chunk> OldChunks = new List<Chunk>(FindObjectsOfType<Chunk>());

        for (int x = 0; x < ChunkNumber.x; x++)
        {
            for (int y = 0; y < ChunkNumber.y; y++)
            {
                for (int z = 0; z < ChunkNumber.z; z++)
                {
                    Vector3Int Coordinate = new Vector3Int(x, y, z);
                    HandleChunkStorage(Coordinate, OldChunks);
                }
            }
        }

        for (int i = 0; i < OldChunks.Count; i++)
        {
            OldChunks[i].DestoryChunk();
        }

    }

    private void HandleChunkStorage(Vector3Int ChunkCord, List<Chunk> OldChunkList)
    {
        bool ChunkExists = false;
        for (int i = 0; i < OldChunkList.Count; i++)
        {
            if (OldChunkList[i].Coordinate == ChunkCord) 
            {
                AllChunks.Add(OldChunkList[i]);
                OldChunkList.RemoveAt(i);

                ChunkExists = true;
                break;
            }
        }

        if(!ChunkExists)
        {
            Chunk NewChunk = CreateChunk(ChunkCord);
            AllChunks.Add(NewChunk);
        }
        AllChunks[AllChunks.Count - 1].ChunkStartup(ChunkMaterial);

    }

    private struct TriangleStruct
    {
#pragma warning disable 649 // disable unassigned variable warning
        public Vector3 PointA;
        public Vector3 PointB;
        public Vector3 PointC;

        public Vector3 GetVertex(int Index)
        {
            switch (Index)
            {
                case 0:
                    return PointA;

                case 1:
                    return PointB;

                default: 
                    return PointC;
            }
        }

    }

}
