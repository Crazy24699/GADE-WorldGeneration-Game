using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using JetBrains.Annotations;

public class Noise 
{
    //public int NoiseScale = 50;
    //public int NoiseOctaves = 6;
    //public int NoiseSeed = 1;

    //public float NoisePersistance = 1.5f;
    //public float Lacunarity = 3;

    //public Vector2 NoiseMapOffset;


    public float[,] GenerateNoiseMap(int MapWidth, int MapHeight, int NoiseSeed, float NoiseScale, int NoiseOctaves, float NoisePersistance, float Lacunarity, Vector2 NoiseMapOffset)
    {
        float[,] noiseMap = new float[MapWidth, MapHeight];
        

        System.Random prng = new System.Random(NoiseSeed);
        Vector2[] octaveOffsets = new Vector2[NoiseOctaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < NoiseOctaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + NoiseMapOffset.x;
            float offsetY = prng.Next(-100000, 100000) - NoiseMapOffset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= NoisePersistance;
        }

        if (NoiseScale <= 0)
        {
            NoiseScale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = MapWidth / 2f;
        float halfHeight = MapHeight / 2f;


        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < NoiseOctaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / NoiseScale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / NoiseScale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    float Noise = Mathf.PerlinNoise(sampleX + 9, sampleY - 18) * 3 - 0.5f;

                    noiseHeight += (perlinValue+Noise) * amplitude;

                    amplitude *= NoisePersistance;
                    frequency *= Lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int x = 0; x < MapHeight; x++)
        {
            for (int y = 0; y < MapWidth; y++)
            {
                float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
            }
        }

        

        return noiseMap;
    }

    public float GetMaxHeight(float[,] Map, int CountLength)
    {
        float MaxValue = -10;

        for (int i = 0; i < CountLength; i++)
        {
            for (int j = 0; j < CountLength; j++)
            {
                Debug.Log(Map[i, j]);
                if (Map[i, j] > MaxValue)
                {
                    MaxValue = Map[i, j];
                }
            }
        }

        return MaxValue;
    }

}
