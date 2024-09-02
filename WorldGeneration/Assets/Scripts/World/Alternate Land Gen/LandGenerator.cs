using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : MonoBehaviour
{
    public int GridSizeX = 20;
    public int GridSizeZ = 20;
    public float GridLength = 1.0f;

    public float NoiseScale = 20f;
    public float NoiseXOffset = 100f;
    public float NoiseYOffset = 100f;

    public float HeightScale = 1f;

    Mesh m_Mesh;

    Vector3[] m_Vertices;
    int[] m_Triangles;

    private void Awake()
    {
        m_Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = m_Mesh;
    }

    private void Update()
    {
        GenerateShape();
        UpdateMesh();
        UpdateMeshCollider();
    }


    private void GenerateShape()
    {
        m_Vertices = new Vector3[(GridSizeX + 1) * (GridSizeZ + 1)];

        int iVertice = 0;
        for (int z = 0; z <= GridSizeZ; z++)
        {
            for (int x = 0; x <= GridSizeX; x++)
            {
                m_Vertices[iVertice] = new Vector3(x * GridLength, GenerateHeight(x, z), z * GridLength);
                iVertice++;
            }
        }

        m_Triangles = new int[GridSizeX * GridSizeZ * 2 * 3];
        int iTriangle = 0;
        for (int row = 0; row < GridSizeX; row++)
        {
            for (int col = 0; col < GridSizeZ; col++)
            {
                int iVertLB = row * (GridSizeX + 1) + col;
                int iVertRB = iVertLB + 1;
                int iVertLT = iVertLB + GridSizeX + 1;
                int iVertRT = iVertLT + 1;

                m_Triangles[iTriangle * 3] = iVertLB;
                m_Triangles[iTriangle * 3 + 1] = iVertLT;
                m_Triangles[iTriangle * 3 + 2] = iVertRB;
                iTriangle++;

                m_Triangles[iTriangle * 3] = iVertLT;
                m_Triangles[iTriangle * 3 + 1] = iVertRT;
                m_Triangles[iTriangle * 3 + 2] = iVertRB;
                iTriangle++;
            }
        }
    }

    private float GenerateHeight(int x, int z)
    {
        float xCoordinate = (float)x / (float)(GridSizeX + 1) * NoiseScale + NoiseXOffset;
        float yCoordinate = (float)z / (float)(GridSizeZ + 1) * NoiseScale + NoiseYOffset;
        return Mathf.PerlinNoise(xCoordinate, yCoordinate) * HeightScale;
    }

    private void UpdateMesh()
    {
        m_Mesh.Clear();
        m_Mesh.vertices = m_Vertices;
        m_Mesh.triangles = m_Triangles;
        m_Mesh.RecalculateNormals();
    }

    private void UpdateMeshCollider()
    {
        GetComponent<MeshCollider>().sharedMesh = m_Mesh;
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < m_Vertices.Length; i++)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawSphere(m_Vertices[i], 0.05f);
    //    }
    //}
}
