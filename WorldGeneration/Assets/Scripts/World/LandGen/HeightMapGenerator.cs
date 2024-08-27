using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator : MonoBehaviour
{
    public HandleNoise NoiseGenerator;
    public MeshGenerator MeshGeneratorScript;

    const int ChunkSize = 241;

    [SerializeField] private float HeightMultiplier;
    [SerializeField] private AnimationCurve HeightCurve;

    public float MinHeight;
    public float MaxHeight;
    public float NoiseScale;

    public int NoiseSeed;
    public int Octaves;
    public int LODValue;

    [Range(0,1)]
    public float Persistance;
    public float Lacunarity;
    public Vector2 offset;

    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public bool AutoUpdate = true;

    public HeightMapValues GenerateHeightMap(int Width, int Height, Vector2 CenterCord)
    {
        Debug.Log("Bang");
        float[,] HeightValues = NoiseGenerator.GenerateNoiseMap(Width, Height, NoiseSeed, NoiseScale, Octaves, Persistance, Lacunarity, Vector2.zero);
        AnimationCurve HeightMapCurve = new AnimationCurve(HeightCurve.keys);

        Debug.Log("Bang Bang");
        float MaxHeightValue = float.MaxValue;
        float MinHeightValue = float.MinValue;

        Debug.Log("Bang Bang Bang");
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                HeightValues[i, j] *= HeightMapCurve.Evaluate(HeightValues[i, j] * HeightMultiplier);
                if (HeightValues[i, j] > MaxHeightValue)
                {
                    MaxHeight = HeightValues[i, j];
                }
                if (HeightValues[i, j] < MinHeightValue)
                {
                    MinHeight = HeightValues[i, j];
                }
            }
        }

        return new HeightMapValues(HeightValues, MinHeight, MaxHeight);
    }

    private void Start()
    {
        GenMap();

    }

    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        //meshRenderer.sharedMaterial.mainTexture = texture;
    }
    public void GenMap()
    {
        HandleNoise NoiseScript = new HandleNoise();
        float[,] NoiseMap = NoiseScript.GenerateNoiseMap(ChunkSize, ChunkSize, NoiseSeed, NoiseScale, Octaves, Persistance, Lacunarity, offset);
        //DrawMesh(MeshGeneratorScript.GenerateTerrainMesh(NoiseMap, HeightMultiplier, HeightCurve, LODValue));
    }

}

public struct HeightMapValues
{
    public readonly float[,] HeightValues;
    public readonly float MinHeightValue;
    public readonly float MaxHeightValue;

    public HeightMapValues(float[,] Values, float MinValue,float MaxValue)
    {
        HeightValues = Values;
        MinHeightValue = MinValue;
        MaxHeightValue = MaxValue;
    }
}
