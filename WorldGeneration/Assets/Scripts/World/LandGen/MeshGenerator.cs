using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
   
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
    private int TrianglesIndex;


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
            OutOfMeshTriangles=
        }
    }

}