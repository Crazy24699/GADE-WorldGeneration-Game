using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleNoise : MonoBehaviour
{
    public enum NoiseNormalization {Local, Global}

    [SerializeField]private NoiseNormalization CurrentNormalizationMode;

    public int NoiseScale = 50;
    public int NoiseOctaves = 6;
    public int NoiseSeed;

    [Range(0, 6)]
    public float NoisePersistance = 1.5f;
    public float Lacunarity = 3;

    public Vector2 NoiseMapOffset;


    public float[,] GenerateNoiseMap(int MapWidth, int MapHeight, Vector2 NoiseCenter)
    {
        float[,] NoiseMap = new float[MapWidth, MapHeight];

        System.Random RandomSeed = new System.Random(NoiseSeed + 1);
        Vector2[] OctaveOffsets = new Vector2[NoiseOctaves];

        float MaxHeight = 0;
        float Amplitude = 1;
        float Frequency = 1;

        for (int i = 0; i < NoiseOctaves; i++)
        {
            float XOffset = RandomSeed.Next(-10000, 10000) + NoiseMapOffset.x + NoiseCenter.x;
            float YOffset = RandomSeed.Next(-10000, 10000) + NoiseMapOffset.y + NoiseCenter.y;

            OctaveOffsets[i] = new Vector2(XOffset, YOffset);
            MaxHeight += Amplitude;
            Amplitude *= NoisePersistance;

        }

        float MaxNoiseHeight = float.MaxValue;
        float MinNoiseHeight = float.MinValue; ;

        float HalfWidth = MapWidth / 2;
        float HalfHeight = MapHeight / 2;

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {

            }
        }

        return NoiseMap;
    }
    

}
