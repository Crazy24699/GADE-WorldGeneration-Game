using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.XR;

public class MapGenerator : MonoBehaviour
{

    public const int MapChunkSize = 239;
    [Range(0, 6)]
    public int EditorPreviewLOD;
    public float NoiseScale;

    public int Octaves;
    [Range(0, 1)]
    public float Persistance;
    public float Lacunarity;

    public int Seed;
    public int ChunkSpawn;

    //How many chunks will spawn on the X Axis
    public int XSpawnNum;

    //How many chunks will spawn on the Y Axis
    public int YSpawnNum;

    public Vector2 Offset;

    public bool UseFalloff;

    public float MeshHeightMultiplier;
    public AnimationCurve MeshHeightCurve;

    public bool AutoUpdate;

    public Noise NoiseGenerator = new Noise();

    public TerrainType[] Regions;


    public bool AutoGen = false;

    [Header("Floats"),Space(2)]
    float[,] FalloffMap;
    public static float MaxViewDst;
    public static GameObject Current;
    #region Const floats
    //const Floats
    const float Scale = 2.5f;
    const float ViewerMoveThresholdForChunkUpdate = 25f;
    const float SqrViewerMoveThresholdForChunkUpdate = ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;
    #endregion

    [Header("Scripts and Structs"),Space(2)]
    //Scripts and Structs
    private MapGenerator MapGeneratorInstance;
    public LODInfoClass[] DetailLevels;
    private TextureGenerator TextureGenScript;
    public static List<Chunk> AllVisableChunks = new List<Chunk>();

    public List<Chunk> CreatedChunks = new List<Chunk>();

    Queue<MapThreadInfo<MapDisplayStruct>> MapDataThreadInfoQueue = new Queue<MapThreadInfo<MapDisplayStruct>>();
    Queue<MapThreadInfo<MeshGenerationData>> MeshDataInfoThread = new Queue<MapThreadInfo<MeshGenerationData>>();


    public Transform Viewer;
    public Material MapMaterial;

    public static Vector2 ViewerPosition;
    Vector2 ViewerPositionOld;


    int ChunkSize;
    int ChunksVisibleInViewDst = 2;

    MeshGenerator MeshGenerator = new MeshGenerator();
    private TextureGenerator TextureGeneratorScript;

    public Dictionary<Vector2, Chunk> SavedChunks = new Dictionary<Vector2, Chunk>();


    void Start()
    {
        MapGeneratorInstance = this;
        MaxViewDst = DetailLevels[DetailLevels.Length - 1].VisibleDstThreshold;
        ChunkSize = MapGenerator.MapChunkSize - 1;
        //ChunksVisibleInViewDst = Mathf.RoundToInt(MaxViewDst / ChunkSize);

        TextureGenScript = new TextureGenerator();

        StartCoroutine(UpdateVisibleChunks());

        SetMeshAndData();

    }

    IEnumerator UpdateVisibleChunks()
    {

        for (int i = 0; i < AllVisableChunks.Count; i++)
        {
            AllVisableChunks[i].SetVisible(false);
        }
        SavedChunks.Clear();

        int CurrentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
        int CurrentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);

        for (int YSpawn = 0; YSpawn < YSpawnNum; YSpawn++)
        {
            for (int XSpawn = 0; XSpawn < XSpawnNum; XSpawn++)
            {
                yield return new WaitForSeconds(0.015f);
                Vector2 ViewedChunkCoord = new Vector2(CurrentChunkCoordX + XSpawn, CurrentChunkCoordY + YSpawn);
                Debug.Log("rite");
                if (SavedChunks.ContainsKey(ViewedChunkCoord))
                {
                    SavedChunks[ViewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    GameObject ChunkObject = new GameObject("terrain" + CreatedChunks.Count);
                    Current = ChunkObject;
                    Chunk MadeChunk = ChunkObject.AddComponent<Chunk>();
                    MadeChunk.SetValues(ViewedChunkCoord, ChunkSize, DetailLevels, transform, MapMaterial);

                    CreatedChunks.Add(MadeChunk);

                    SavedChunks.Add(ViewedChunkCoord, MadeChunk);

                }
            }
        }
    }

    public void DrawMapInEditor()
    {
        MapDisplayStruct MapData = GenerateMapData(Vector2.zero);

        MapDisplay MapDisplayRef = FindObjectOfType<MapDisplay>();
        MapDisplayRef.DrawMesh(MeshGenerator.GenerateTerrainMesh(MapData.HeightMapValues, MeshHeightMultiplier, MeshHeightCurve, EditorPreviewLOD), TextureGenerator.ColourMapTextureGen(MapData.ColourMapValues, MapChunkSize, MapChunkSize));
        //ChunkSpawn++;
        //Debug.Log(ChunkSpawn);
    }
    public void RequestMapData(Vector2 ChunkCenter, Action<MapDisplayStruct> MapDisplayRef)
    {
        ThreadStart ThreadScriptStart = delegate {
            MapDataThread(ChunkCenter, MapDisplayRef);
        };

        new Thread(ThreadScriptStart).Start();
    }

    void MapDataThread(Vector2 MapCenterCord, Action<MapDisplayStruct> MapDisplayRef)
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

    void MeshDataThread(MapDisplayStruct MapDisplayRef, int LODValue, Action<MeshGenerationData> MeshGenDataRef)
    {
        //where its generated. Look here

        MeshGenerationData MeshData = MeshGenerator.GenerateTerrainMesh(MapDisplayRef.HeightMapValues, MeshHeightMultiplier, MeshHeightCurve, LODValue);
        
        

        Debug.Log("right");
        lock (MeshDataInfoThread)
        {
            //ChunkSpawn++;
            //Debug.Log(ChunkSpawn);
            //Seed++;
            
            MeshDataInfoThread.Enqueue(new MapThreadInfo<MeshGenerationData>(MeshGenDataRef, MeshData));
            Debug.Log(MeshData.BorderVertices.Length + "     " + MeshData.VertsPerLine);
        }
    }

    void Update()
    {
        if (MapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < MapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapDisplayStruct> threadInfo = MapDataThreadInfoQueue.Dequeue();
                threadInfo.Callback(threadInfo.Parameter);
            }
        }

        if (MeshDataInfoThread.Count > 0)
        {
            for (int i = 0; i < MeshDataInfoThread.Count; i++)
            {
                MapThreadInfo<MeshGenerationData> ThreadInfo = MeshDataInfoThread.Dequeue();
                ThreadInfo.Callback(ThreadInfo.Parameter);
            }
        }
    }

    private void SetMeshAndData()
    {
  
    }

    MapDisplayStruct GenerateMapData(Vector2 MapCenterCoord)
    {

        float[,] GeneratedNoiseMap = NoiseGenerator.GenerateNoiseMap(MapChunkSize + 2, MapChunkSize + 2, Seed, NoiseScale, Octaves, Persistance, Lacunarity, MapCenterCoord + Offset);

        Color[] GeneratedColourMap = new Color[MapChunkSize * MapChunkSize];
        for (int y = 0; y < MapChunkSize; y++)
        {
            for (int x = 0; x < MapChunkSize; x++)
            {
                if (UseFalloff)
                {
                    GeneratedNoiseMap[x, y] = Mathf.Clamp01(GeneratedNoiseMap[x, y] - FalloffMap[x, y]);
                }
                float currentHeight = GeneratedNoiseMap[x, y];
                for (int i = 0; i < Regions.Length; i++)
                {
                    if (currentHeight >= Regions[i].Height)
                    {
                        GeneratedColourMap[y * MapChunkSize + x] = Regions[i].Colour;
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

        //FalloffMap = FalloffGenerator.GenerateFalloffMap(MapChunkSize);
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
    private int LODValue;
    private System.Action UpdateMeshLOD;
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
            MadeChunk.GetComponent<ChunkInfo>().Populate();
            
        }

        UpdateMeshLOD();
    }

    public void RequestMesh(MapDisplayStruct MapDisplayData)
    {
        HasRequestedMesh = true;
        MapGenScript.RequestMeshData(MapDisplayData, LODValue, OnMeshDataReceived);
        //GeneratedMesh.GetComponent<ChunkInfo>().Meshdata =;
    }



}

[System.Serializable]
public struct TerrainType
{
    public string Name;
    public float Height;
    public Color Colour;
}

public struct MapDisplayStruct
{
    public readonly float[,] HeightMapValues;
    public readonly Color[] ColourMapValues;

    public MapDisplayStruct(float[,] heightMap, Color[] colourMap)
    {
        HeightMapValues = heightMap;
        ColourMapValues = colourMap;
    }
}
