using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator : MonoBehaviour
{
    public HandleNoise NoiseGenerator;

    [SerializeField] private float HeightMultiplier;
    [SerializeField] private AnimationCurve HeightCurve;

    public float MinHeight;
    public float MaxHeight;

    public HeightMapValues GenerateHeightMap(int Width, int Height, Vector2 CenterCord)
    {
        Debug.Log("Bang");
        float[,] HeightValues = NoiseGenerator.GenerateNoiseMap(Width, Height, CenterCord);
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
