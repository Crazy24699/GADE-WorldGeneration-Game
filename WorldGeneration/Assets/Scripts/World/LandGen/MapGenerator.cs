using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int ColliderLODIndex;
    
    public float MeshWorldSize;

    public LODInfo[] LevelDetails;

    public MeshGenerator MeshGenScript;
    public HeightMapGenerator HeightMapGeneratorScript;

    public Material MapMaterialRef;

    public Vector2 MeshGenerationParameters;
    private Dictionary<Vector2, MapChunk> SavedChunks = new Dictionary<Vector2, MapChunk>();
    private List<MapChunk> VisableChunks = new List<MapChunk>();

    public bool AutoUpdate = true;

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

    public void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = VisableChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(VisableChunks[i].ChunkCoordinate);
            VisableChunks[i].UpdateChunk();
        }

        for (int yOffset = 0; yOffset <= MeshGenerationParameters.x; yOffset++)
        {
            for (int xOffset = 0; xOffset <= MeshGenerationParameters.y; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (SavedChunks.ContainsKey(viewedChunkCoord))
                    {
                        SavedChunks[viewedChunkCoord].UpdateChunk();
                    }
                    else
                    {
                        MapChunk NewChunk = new MapChunk(viewedChunkCoord, new HeightMapGenerator(), new MeshGenerator(), LevelDetails, ColliderLODIndex, transform, MapMaterialRef);
                        SavedChunks.Add(viewedChunkCoord, NewChunk);
                        //newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        //NewChunk.LoadChunk();
                    }
                }

            }
        }


    }


    public void UpdateVisibleChunkss()
    {
        HashSet<Vector2> UpdatedChunkCoords = new HashSet<Vector2>();
        
        for (int i = 0; i < VisableChunks.Count; i++)
        {
            UpdatedChunkCoords.Add(VisableChunks[i].ChunkCoordinate);
            VisableChunks[i].UpdateChunk();
        }

        for (int XOffset = 0; XOffset < MeshGenerationParameters.x; XOffset++)
        {
            for (int YOffset = 0; YOffset < MeshGenerationParameters.y; YOffset++)
            {
                Vector2 ChunkCord = new Vector2(XOffset, YOffset);
                MapChunk NewChunk = new MapChunk(ChunkCord, new HeightMapGenerator(), new MeshGenerator(), LevelDetails, ColliderLODIndex, transform, MapMaterialRef);
                if (!UpdatedChunkCoords.Contains(ChunkCord))
                {
                    Debug.Log("run");
                    if (SavedChunks.ContainsKey(ChunkCord))
                    {
                        SavedChunks[ChunkCord].UpdateChunk();
                    }
                    else
                    {
                        //MapChunk NewChunk = new MapChunk(ChunkCord, HeightMapGeneratorScript, MeshGenScript, LevelDetails, ColliderLODIndex, transform, MapMaterialRef);
                        SavedChunks.Add(ChunkCord, NewChunk);
                        //NewChunk.LoadChunk();
                    }
                }
            }
        }


    }

    /*
    
     
     */

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
