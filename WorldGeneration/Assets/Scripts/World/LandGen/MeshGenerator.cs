using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public float MeshScale = 2.5f;
    public float MeshWorldScale;
    public bool FlatShadingActive;

    public const int AllLODLevels = 5;
    public const int MaxChunkSize = 9;
    public const int MaxFlatshadedChunkSize = 3;
    public int VerticesPerLine;
    public static readonly int[] SpecifiedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    [Range(0, MaxChunkSize - 1)]
    public int ChunkSizeIndex;
    [Range(0, MaxFlatshadedChunkSize - 1)]
    public int FlatshadedChunkSizeIndex;

    private MeshGenerator()
    {
        VerticesPerLine = SpecifiedChunkSizes[(FlatShadingActive) ? FlatshadedChunkSizeIndex : ChunkSizeIndex] + 5;
        MeshWorldScale = (VerticesPerLine - 3) * MeshScale;
    }

}

public class MeshGenerationData
{
    private Vector3[] Vertices;
    private Vector2[] UVs;
    private Vector3[] BakedNormals;
    private Vector3[] OutOfMeshVertices;

    private int[] Triangles;
    private int TriangleIndex;
    
    private int[] OutOfMeshTriangles;
    private int OutOfMeshTriangleIndex;


    private bool FlatShadingActive;

    public MeshGenerationData(int BaseVerticesPerLine, int SkipIncriment, bool UsingFlatShading)
    {
        FlatShadingActive = UsingFlatShading;
        int MeshEdgeVertixes = (BaseVerticesPerLine - 2) * 4 - 4;
        int ConnectionVertices = (SkipIncriment - 1) * (BaseVerticesPerLine - 3) / SkipIncriment * 4;
        int VerticesPerLine = (BaseVerticesPerLine - 5) / SkipIncriment + 1;
        int VerticesCount = VerticesPerLine ^ 2;

        Vertices = new Vector3[MeshEdgeVertixes + ConnectionVertices + VerticesCount];
        UVs = new Vector2[Vertices.Length];

        int TriangleEdges = 8 * (BaseVerticesPerLine - 4);
        int BaseTriangleCount = (VerticesCount - 1) * (VerticesCount - 1) * 2;
        Triangles = new int[(TriangleEdges + BaseTriangleCount) * 3];

        OutOfMeshVertices = new Vector3[BaseVerticesPerLine * 4 - 4];
        OutOfMeshTriangles = new int[24 * (BaseVerticesPerLine - 2)];

    }

    public void AddVertex(Vector3 VertexPosition, Vector2 VertexUV, int VertexIndex)
    {
        if (VertexIndex < 0)
        {
            OutOfMeshVertices[-VertexIndex - 1] = VertexPosition;
        }
        else
        {
            Vertices[VertexIndex] = VertexPosition;
            UVs[VertexIndex] = VertexUV;
        }
    }

    public void AddTriangle(int PointA, int PointB, int PointC)
    {
        if (PointA < 0 || PointB < 0 || PointC < 0) 
        {
            OutOfMeshTriangles[OutOfMeshTriangleIndex] = PointA;
            OutOfMeshTriangles[OutOfMeshTriangleIndex + 1] = PointB;
            OutOfMeshTriangles[OutOfMeshTriangleIndex + 2] = PointC;
            OutOfMeshTriangleIndex += 3;
        }
        else
        {
            Triangles[TriangleIndex] = PointA;
            Triangles[TriangleIndex+1] = PointB;
            Triangles[TriangleIndex+2] = PointC;
            TriangleIndex += 3;
        }
    }

    private Vector3[] CalculateNormals()
    {
        Vector3[] VertexNormals = new Vector3[Vertices.Length];
        int TriangleCount = Triangles.Length / 3;

        for (int i = 0; i < TriangleCount; i++)
        {
            int TriangleNormalizedIndex = i * 3;
            int VertexIndexA = Triangles[TriangleNormalizedIndex];
            int VertexIndexB = Triangles[TriangleNormalizedIndex + 1];
            int VertexIndexC = Triangles[TriangleNormalizedIndex + 2];

            Vector3 TriangleNormal = CalculateSurfaceNormal(VertexIndexA, VertexIndexB, VertexIndexC);
            if (VertexIndexA >= 0)
            {
                VertexNormals[VertexIndexA] += TriangleNormal;
            }
            if (VertexIndexB >= 0)
            {
                VertexNormals[VertexIndexB] += TriangleNormal;
            }
            if (VertexIndexB >= 0)
            {
                VertexNormals[VertexIndexB] += TriangleNormal;
            }

        }

        for (int i = 0; i < VertexNormals.Length; i++)
        {
            VertexNormals[i].Normalize();
        }
        return VertexNormals;
    }

    private Vector3 CalculateSurfaceNormal(int IndexA, int IndexB, int IndexC)
    {
        Vector3 PointA = (IndexA < 0) ? OutOfMeshVertices[-IndexA - 1] : Vertices[IndexA];
        Vector3 PointB = (IndexB < 0) ? OutOfMeshVertices[-IndexB - 1] : Vertices[IndexB];
        Vector3 PointC = (IndexC < 0) ? OutOfMeshVertices[-IndexC - 1] : Vertices[IndexC];

        Vector3 SideAB = PointB - PointA;
        Vector3 SideAC = PointC - PointA;

        return Vector3.Cross(SideAB, SideAC).normalized;
    }

    private void BakeNormals()
    {
        BakedNormals = CalculateNormals();
    }

    public void MeshProcessing()
    {
        if (FlatShadingActive)
        {
            ApplyFlatShading();
        }
        else
        {
            BakeNormals();
        }
    }

    private void ApplyFlatShading()
    {
        Vector3[] FlatShadedVertices = new Vector3[Triangles.Length];
        Vector2[] FlatShadedUVs = new Vector2[Triangles.Length];

        for (int i = 0; i < Triangles.Length; i++)
        {
            FlatShadedVertices[i] = Vertices[Triangles[i]];
            FlatShadedUVs[i] = UVs[Triangles[i]];
            Triangles[i] = i;
        }

        Vertices = FlatShadedVertices;
        UVs = FlatShadedUVs;

    }

    public Mesh CreateMesh()
    {
        Mesh MeshRef = new Mesh();
        MeshRef.vertices = Vertices;
        MeshRef.triangles = Triangles;
        MeshRef.uv = UVs;

        if(FlatShadingActive)
        {
            MeshRef.RecalculateNormals();
        }
        else
        {
            MeshRef.normals = BakedNormals;
        }
        return MeshRef;
    }

}