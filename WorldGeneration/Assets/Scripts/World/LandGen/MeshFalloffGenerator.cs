using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFalloffGenerator 
{

    public static float[,] FalloffMapGenerator(int MapSize)
    {
        float[,] MapRef = new float[MapSize, MapSize];

        for (int i = 0; i < MapSize; i++)
        {
            for (int j = 0; j < MapSize; j++)
            {
                float XValue = i / (float)MapSize * 2 - 1;
                float YValue = j / (float)MapSize * 2 - 1;

                float MapValue = Mathf.Max(Mathf.Abs(XValue), Mathf.Abs(YValue));
                MapRef[i,j] = EvaluateValue(MapValue);
            }
        }
        
        return MapRef;
    }

    private static float EvaluateValue(float Value)
    {
        float ValueA = 3;
        float ValueB = 2.25f;

        return Mathf.Pow(Value, ValueA) / (Mathf.Pow(Value, ValueA) + Mathf.Pow(ValueB - ValueB * Value, ValueA));
    }
    
}
