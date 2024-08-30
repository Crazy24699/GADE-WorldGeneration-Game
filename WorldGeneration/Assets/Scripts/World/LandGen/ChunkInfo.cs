using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkInfo : MonoBehaviour
{
    public MeshGenerationData Meshdata;

    public Vector3[] BorderVertices;
    public int[] BorderTriangles;

    public bool Populated = false;

    public void PopulateInfo(Vector3[] SetVertices, int[] SetTriangles)
    {
        BorderVertices = SetVertices;
        BorderTriangles = SetTriangles;
    }
    public void Populate()
    {
        BorderVertices = Meshdata.BorderVertices;
        BorderTriangles = Meshdata.BorderTriangles;

        Populated = true;
    }

    private void OnDrawGizmos()
    {
        if (!Populated)
        {
            return;
        }

        foreach (var Cord in BorderVertices)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(Cord, 1.25f);
            //Debug.Log("true");
        }
    }

}
