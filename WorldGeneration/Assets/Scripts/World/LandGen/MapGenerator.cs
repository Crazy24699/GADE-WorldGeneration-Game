using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.XR;

public class MapGenerator : MonoBehaviour
{
    //Floats
    [Header("Floats"),Space(2)]
    public float NoiseScale;
    [Range(0, 1)]
    public float Persistance;

    private float[,] FalloffMap;
    public static float MaxViewDst;
    public float MeshHeightMultiplier;
    public float Lacunarity;


    //Ints
    public const int MapChunkSize = 239;
    public int Octaves;
    public int Seed;
    [Range(0, 6)]
    public int EditorPreviewLOD;
    public int ChunkSpawn;
    //How many chunks will spawn on the X Axis
    public int XSpawnNum;
    //How many chunks will spawn on the Y Axis
    public int YSpawnNum;
    public int ChunkSize;


    //Bools
    public bool AutoGen = false;
    [HideInInspector]public bool MapGenerated=false;

    //Vectors
    public Vector2 Offset;
    public static Vector2 ViewerPosition;
    public Dictionary<Vector2, Chunk> SavedChunks = new Dictionary<Vector2, Chunk>();

    //Game obejcts and transfroms
    public static GameObject Current;
    public GameObject BasePlane;
    public Transform Viewer;


    [Header("Scripts and Structs"),Space(2)]
    //Scripts and Structs
    public static List<Chunk> AllVisableChunks = new List<Chunk>();
    private MeshGenerator MeshGenerator = new MeshGenerator();
    public List<Chunk> CreatedChunks = new List<Chunk>();
    public Noise NoiseGenerator = new Noise();

    Queue<MapThreadInfo<MapDisplayStruct>> MapDataThreadInfoQueue = new Queue<MapThreadInfo<MapDisplayStruct>>();
    Queue<MapThreadInfo<MeshGenerationData>> MeshDataInfoThread = new Queue<MapThreadInfo<MeshGenerationData>>();
    
    public LODInfoClass[] DetailLevels;
    public TerrainType[] Regions;
    
    //Meshes
    public MeshFilter GeneratedMesh;
    public Material MapMaterial;
    public AnimationCurve MeshHeightCurve;


    void Start()
    {
        Debug.Log("undone");
        RandomizeTerrain();

        MaxViewDst = DetailLevels[DetailLevels.Length - 1].VisibleDstThreshold;
        ChunkSize = MapGenerator.MapChunkSize - 1;

        UpdateVisibleChunks();
        BasePlane.SetActive(false);
    }

    private void RandomizeTerrain()
    {
        int RandomSeedChange = UnityEngine.Random.Range(-10, 50);
        Seed += Seed + RandomSeedChange;
        Offset.x = UnityEngine.Random.Range(-50, 50);
        Offset.x = UnityEngine.Random.Range(-10, 60);

    }

    private void UpdateVisibleChunks()
    {

        for (int i = 0; i < AllVisableChunks.Count; i++)
        {
            AllVisableChunks[i].SetVisible(false);
        }
        //SavedChunks.Clear();

        int CurrentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
        int CurrentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);

        for (int YSpawn = 0; YSpawn < YSpawnNum; YSpawn++)
        {
            for (int XSpawn = 0; XSpawn < XSpawnNum; XSpawn++)
            {
                
                Vector2 ViewedChunkCoord = new Vector2(CurrentChunkCoordX + XSpawn, CurrentChunkCoordY + YSpawn);

                if (SavedChunks.ContainsKey(ViewedChunkCoord))
                {
                    SavedChunks[ViewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    GameObject ChunkObject = new GameObject("terrain" + SavedChunks.Count);
                    ChunkObject.layer = LayerMask.NameToLayer("Ground");
                    Current = ChunkObject;
                    Chunk MadeChunk = ChunkObject.AddComponent<Chunk>();

                    //Debug.Log("ChunkObject"+ChunkObject.name);
                    MadeChunk.SetValues(ViewedChunkCoord, ChunkSize, DetailLevels, transform, MapMaterial);

                    //CreatedChunks.Add(MadeChunk);

                    SavedChunks.Add(ViewedChunkCoord, MadeChunk);
                    GeneratedMesh = MadeChunk.MeshFilterRef;
                }
            }
        }

        MapGenerated = true;
    }

    public void DrawMapInEditor()
    {
        MapDisplayStruct MapData = GenerateMapData(Vector2.zero);

        MapDisplay MapDisplayRef = FindObjectOfType<MapDisplay>();
        MapDisplayRef.DrawMesh(MeshGenerator.GenerateTerrainMesh(MapData.HeightMapValues, MeshHeightMultiplier, MeshHeightCurve, EditorPreviewLOD), TextureGenerator.ColourMapTextureGen(MapData.ColourMapValues, MapChunkSize, MapChunkSize));

    }
    public void RequestMapData(Vector2 ChunkCenter, Action<MapDisplayStruct> MapDisplayRef)
    {
        ThreadStart ThreadScriptStart = delegate {
            MapDataThread(ChunkCenter, MapDisplayRef);
        };

        new Thread(ThreadScriptStart).Start();
    }

    private void MapDataThread(Vector2 MapCenterCord, Action<MapDisplayStruct> MapDisplayRef)
    {
        MapDisplayStruct MapDisplayData = GenerateMapData(MapCenterCord);
        lock (MapDataThreadInfoQueue)
        {
            MapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapDisplayStruct>(MapDisplayRef, MapDisplayData));
        }
    }

    public void RequestMeshData(MapDisplayStruct MapDisplayRef, int LODValue, Action<MeshGenerationData> MeshGenDataRef)
    {
        ThreadStart ThreadScriptStart = delegate {
            MeshDataThread(MapDisplayRef, LODValue, MeshGenDataRef);
        };

        new Thread(ThreadScriptStart).Start();
    }

    private void MeshDataThread(MapDisplayStruct MapDisplayRef, int LODValue, Action<MeshGenerationData> MeshGenDataRef)
    {
        //where its generated. Look here

        MeshGenerationData MeshData = MeshGenerator.GenerateTerrainMesh(MapDisplayRef.HeightMapValues, MeshHeightMultiplier, MeshHeightCurve, LODValue);

        //Debug.Log("right");
        lock (MeshDataInfoThread)
        {
            
            MeshDataInfoThread.Enqueue(new MapThreadInfo<MeshGenerationData>(MeshGenDataRef, MeshData));
        }
    }


    void Update()
    {
        HandleThreading();
        
    }

    private void HandleThreading()
    {
        if (MapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < MapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapDisplayStruct> ThreadInfoRef = MapDataThreadInfoQueue.Dequeue();
                ThreadInfoRef.Callback(ThreadInfoRef.Parameter);
            }
        }

        if (MeshDataInfoThread.Count > 0)
        {
            for (int i = 0; i < MeshDataInfoThread.Count; i++)
            {
                MapThreadInfo<MeshGenerationData> ThreadInfoRef = MeshDataInfoThread.Dequeue();
                ThreadInfoRef.Callback(ThreadInfoRef.Parameter);
            }
        }
    }

    private MapDisplayStruct GenerateMapData(Vector2 MapCenterCoord)
    {

        float[,] GeneratedNoiseMap = NoiseGenerator.GenerateNoiseMap(MapChunkSize + 2, MapChunkSize + 2, Seed, NoiseScale, Octaves, Persistance, Lacunarity, MapCenterCoord + Offset);

        Color[] GeneratedColourMap = new Color[MapChunkSize * MapChunkSize];
        for (int y = 0; y < MapChunkSize; y++)
        {
            for (int x = 0; x < MapChunkSize; x++)
            {
                float currentHeight = GeneratedNoiseMap[x, y];
                for (int i = 0; i < Regions.Length; i++)
                {
                    if (currentHeight >= Regions[i].Height)
                    {
                        //GeneratedColourMap[y * MapChunkSize + x] = Regions[i].Colour;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapDisplayStruct(GeneratedNoiseMap, GeneratedColourMap);
    }

    void OnValidate()
    {
        if (Lacunarity < 1)
        {
            Lacunarity = 1;
        }

        if (Octaves < 0)
        {
            Octaves = 0;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> Callback;
        public readonly T Parameter;

        public MapThreadInfo(Action<T> MapThreadAction, T ParsedParameter)
        {
            Callback = MapThreadAction;
            Parameter = ParsedParameter;
        }
    }

}
    [System.Serializable]
public struct LODInfoClass
{
    public int Lod;
    public float VisibleDstThreshold;
    public bool UseForCollider;
}

public class LODMeshClass
{
    public GameObject MadeChunk;
    public Mesh MeshRef;

    public bool HasRequestedMesh;
    public bool HasMesh;
    
    //The LOD that the mesh will be rendered in
    private int LODValue;

    //Actions used for threading and to update the mesh
    private System.Action UpdateMeshLOD;

    //Reference to the script above, needed for requesting the mesh and generating it
    private MapGenerator MapGenScript;

    public LODMeshClass(int LodVar, System.Action UpdateCallbackVar, MapGenerator MapGeneratorScript)
    {
        LODValue = LodVar;
        UpdateMeshLOD = UpdateCallbackVar;
        MapGenScript = MapGeneratorScript;

    }

    void OnMeshDataReceived(MeshGenerationData MeshDataVar)
    {
        MeshRef = MeshDataVar.CreateMesh();
        HasMesh = true;

        if (MadeChunk != null)
        {
            MadeChunk.GetComponent<ChunkInfo>().Meshdata = MeshDataVar;
            //MadeChunk.GetComponent<ChunkInfo>().HandleMeshInfo();

        }

        UpdateMeshLOD();
    }

    public void RequestMesh(MapDisplayStruct MapDisplayData)
    {
        HasRequestedMesh = true;
        MapGenScript.RequestMeshData(MapDisplayData, LODValue, OnMeshDataReceived);
    }
}

[System.Serializable]
public struct TerrainType
{
    public string Name;
    public float Height;
    public Color Colour;
}


public class MapDisplayStruct
{
    public readonly float[,] HeightMapValues;
    public readonly Color[] ColourMapValues;

    public MapDisplayStruct(float[,] heightMap, Color[] colourMap)
    {
        HeightMapValues = heightMap;
        //ColourMapValues = colourMap;
    }
}
