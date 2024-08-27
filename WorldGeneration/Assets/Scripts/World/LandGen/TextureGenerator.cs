using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{

    private const int TextureSize = 512;
    private const TextureFormat textureFormat = TextureFormat.RGB565;

    public Layer[] AllLayers;

    private float SavedMaxHeight;
    private float SavedMinHeight;

    public void ApplyToMaterial(Material MaterialRef)
    {

    }

}

[System.Serializable]
public class Layer
{
    public Texture2D AppliedTexture;
    public Color TextureTint;
    [Range(0, 1)]
    public float TintStrength;
    [Range(0,1)]
    public float StartingHeight;
    [Range(0, 1)]
    public float BlendStrnStrength;
    public float TextureScale;

}