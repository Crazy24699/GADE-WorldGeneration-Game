using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkInfo : MonoBehaviour
{
    public Vector3[] BorderVertices;
    public int[] BorderTriangles;

    public void PopulateInfo(Vector3[] SetVertices, int[] SetTriangles)
    {
        BorderVertices = SetVertices;
        BorderTriangles = SetTriangles;
    }

}
