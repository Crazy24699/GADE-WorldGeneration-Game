using UnityEngine;

public class TextureGenerator 
{
    public static Texture2D ColourMapTextureGen(Color[] ColourMap, int TextureWidth, int TextureHeight)
    {
        Texture2D Texture2DRef = new Texture2D(TextureWidth, TextureHeight);
        //Texture2DRef.filterMode = FilterMode.Point;
        //Texture2DRef.wrapMode = TextureWrapMode.Clamp;

        //Texture2DRef.SetPixels(ColourMap);
        //Texture2DRef.Apply();

        return Texture2DRef;
    }

    public static Texture2D HeightMapTextureGen(float[,] HeightMapValue)
    {
        int WidthValue = HeightMapValue.GetLength(0);
        int HeightValue = HeightMapValue.GetLength(1);

        Color[] ColourMap = new Color[WidthValue * HeightValue];

        for (int y = 0; y < HeightValue; y++)
        {
            for (int x = 0; x < WidthValue; x++)
            {
                ColourMap[y * WidthValue + x] = Color.Lerp(Color.black, Color.white, HeightMapValue[x, y]);
            }
        }

        return ColourMapTextureGen(ColourMap, WidthValue, HeightValue);
    }

}
