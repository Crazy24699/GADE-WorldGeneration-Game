using UnityEngine;

public class Noise 
{
    //public int NoiseScale = 50;
    //public int NoiseOctaves = 6;
    //public int NoiseSeed = 1;

    //public float NoisePersistance = 1.5f;
    //public float Lacunarity = 3;

    //public Vector2 NoiseMapOffset;

    private int MapHeight;
    private int MapWidth;

    private float HalfMapWidth;
    private float HalfMapHeight;

    public float[,] GenerateNoiseMap(int MapWidthValue, int MapHeightValue, int NoiseSeed, float NoiseScale, int NoiseOctaves, float NoisePersistance, float Lacunarity, Vector2 NoiseMapOffset)
    {
        float[,] BlankNoiseMap = new float[MapWidthValue, MapHeightValue];

        MapHeight = MapHeightValue;
        MapWidth = MapWidthValue;

        System.Random prng = new System.Random(NoiseSeed);
        Vector2[] OctaveOffset = new Vector2[NoiseOctaves];

        float MaxPossibleHeight = 0;
        float NoiseAmplitude = 1;

        // Generating octave offsets and maximum possible height
        for (int i = 0; i < NoiseOctaves; i++)
        {
            float X_Offset = prng.Next(-100000, 100000) + NoiseMapOffset.x;
            float Y_Offset = prng.Next(-100000, 100000) - NoiseMapOffset.y;
            OctaveOffset[i] = new Vector2(X_Offset, Y_Offset);

            MaxPossibleHeight += NoiseAmplitude;
            NoiseAmplitude *= NoisePersistance;
        }

        if (NoiseScale <= 0)
        {
            NoiseScale = 0.0001f;
        }

        HalfMapWidth = MapWidth / 2f;
        HalfMapHeight = MapHeight / 2f;

        float MaxNoiseHeight = float.MinValue;
        float MinNoiseHeight = float.MaxValue;

        BlankNoiseMap = SetNoiseHeight(NoiseScale, NoiseOctaves, OctaveOffset, Lacunarity, NoisePersistance, BlankNoiseMap, ref MaxNoiseHeight, ref MinNoiseHeight);

        // Normalize noise heights
        for (int y = 0; y < MapHeightValue; y++)
        {
            for (int x = 0; x < MapWidthValue; x++)
            {
                BlankNoiseMap[y, x] = Mathf.InverseLerp(MaxNoiseHeight, MinNoiseHeight, BlankNoiseMap[y, x]);
            }
        }



        return BlankNoiseMap;
    }

    public float[,] SetNoiseHeight(float NoiseScale, int NoiseOctaves, Vector2[] OffsetOctaves,float NoiseLacunarity, float NoisePersistance, float[,] NoiseMapValues,ref float MaxNoiseHeight,ref float MinNoiseHeight)
    {

        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                float NoiseAmplitude = 1;
                float NoiseFrequency = 1;

                float NoiseHeight = 0;

                for (int i = 0; i < NoiseOctaves; i++)
                {
                    float XNoiseValue = (x - HalfMapWidth + OffsetOctaves[i].x) / NoiseScale * NoiseFrequency;
                    float YNoiseValue = (y - HalfMapHeight + OffsetOctaves[i].y) / NoiseScale * NoiseFrequency;

                    float PerlinNoiseValue = Mathf.PerlinNoise(XNoiseValue, YNoiseValue) * 2 - 1;
                    NoiseHeight += PerlinNoiseValue * NoiseAmplitude;

                    NoiseAmplitude *= NoisePersistance;
                    NoiseFrequency *= NoiseLacunarity;
                }

                if (NoiseHeight > MaxNoiseHeight)
                {
                    MaxNoiseHeight = NoiseHeight;
                }
                if (NoiseHeight < MinNoiseHeight)
                {
                    MinNoiseHeight = NoiseHeight;
                }

                NoiseMapValues[y, x] = NoiseHeight;
            }
        }

        return NoiseMapValues;
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
