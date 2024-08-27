using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{


    public const int MapChunkSize = 240;
    public int ColliderLODIndex;
    public int NoiseSeed;
    public int Octaves;
    public int LODValue;

    [Space(2), Header("Floats")]
    public float MinHeight;
    #region Floats  
    public float MaxHeight;
    public float NoiseScale;
    public float MeshWorldSize;
    public float HeightMultiplyer;
    [Space(2)]
    [Range(0, 1)]
    public float Persistance;
    public float Lacunarity;
    #endregion

    private float[,] FalloffMap;


    public Vector2 MapOffset;
    public Vector2 MeshGenerationParameters;

    public bool AutoUpdate = true;

    public LODInfo[] LevelDetails;

    public MeshGenerator MeshGenScript;
    public HeightMapGenerator HeightMapGeneratorScript;
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Material MapMaterialRef;
    public AnimationCurve HeightCurve;

    private Dictionary<Vector2, MapChunk> SavedChunks = new Dictionary<Vector2, MapChunk>();
    private List<MapChunk> VisableChunks = new List<MapChunk>();

    private Queue<MapThreadInfo<MapData>> MapDataThreadQueue = new Queue<MapThreadInfo<MapData>>();
    private Queue<MapThreadInfo<MeshData>> MeshDataThreadQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Awake()
    {
        FalloffMap = MeshFalloffGenerator.FalloffMapGenerator(MapChunkSize);
    }

    private void Start()
    {
        if (MeshGenerationParameters == Vector2.zero)
        {
            MeshGenerationParameters = Vector2.one;
            MeshGenerationParameters = Vector2.one;
        }

        //UpdateVisibleChunks();
    }

    public void GenerateMap()
    {
        //float[,] NoiseMap=new HandleNoise().GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset)

    }

    #region Threading
    private void RequestMapData(Vector2 MapCenter,Action<MapData> MapDataVar)
    {
        ThreadStart ThreadStartScript = delegate
        {
            MapDataThread(MapCenter, MapDataVar);
        };
        new Thread(ThreadStartScript).Start();
    }

    private void MapDataThread(Vector2 MapCenter, Action<MapData> MapDataVar)
    {
        MapData MapDataRef = GenerateMapData(MapCenter);
        lock (MapDataThreadQueue)
        {
            MapDataThreadQueue.Enqueue(new MapThreadInfo<MapData>(MapDataVar, MapDataRef));
        }

    }

    public void RequestMeshData(MapData MapDataVar,int LOD, Action<MeshData> MeshDataVar)
    {
        ThreadStart ThreadStartScript = delegate
        {
            MeshDataThread(MapDataVar, LOD, MeshDataVar);
        };
        new Thread(ThreadStartScript).Start();
    }

    public void MeshDataThread(MapData MapDataRef, int LOD, Action<MeshData> MeshDataVar)
    {
        MeshData MeshDataScript = MeshGenerator.GenerateTerrainMesh(MapDataRef.HeightMapData, HeightMultiplyer, HeightCurve, LOD);
        lock (MeshDataThreadQueue)
        {
            MeshDataThreadQueue.Enqueue(new MapThreadInfo<MeshData>(MeshDataVar, MeshDataScript));
        }
    }

    private MapData GenerateMapData(Vector2 MapCenter)
    {
        float[,] NoiseMap = new HandleNoise().GenerateNoiseMap(MapChunkSize, MapChunkSize, NoiseSeed, NoiseScale, Octaves, Persistance, Lacunarity, MapOffset);

        Color[] ColourMap = new Color[MapChunkSize ^ 2];
        for (int x = 0; x < MapChunkSize; x++)
        {
            for (int y = 0; y < MapChunkSize; y++)
            {
                NoiseMap[x, y] = Mathf.Clamp01(NoiseMap[x, y] - FalloffMap[x, y]);
                float CurrentHeight = NoiseMap[x, y];
            }
            

        }

        return new MapData(NoiseMap, ColourMap);
    }
    #endregion

    private void OnValidate()
    {

        FalloffMap = MeshFalloffGenerator.FalloffMapGenerator(MapChunkSize);
    }

    private struct MapThreadInfo<T>
    {
        public readonly Action<T> AssignedAction;
        public readonly T SetParameter;

        public MapThreadInfo(Action<T> ParsedAction, T ParsedParameter)
        {
            AssignedAction = ParsedAction;
            SetParameter = ParsedParameter;
        }

    }

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

public struct MapData
{
    public readonly float[,] HeightMapData;
    public readonly Color[] ColourMapData;

    public MapData(float[,] HeightMapRef, Color[] ColourMapRef)
    {
        HeightMapData = HeightMapRef;
        ColourMapData = ColourMapRef;
    }

}
