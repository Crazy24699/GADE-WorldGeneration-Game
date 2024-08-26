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

    private int VerticesIndex = 0;
    private int OutOfMeshIndex = -1;

    [Range(0, MaxChunkSize - 1)]
    public int ChunkSizeIndex;
    [Range(0, MaxFlatshadedChunkSize - 1)]
    public int FlatshadedChunkSizeIndex;

    private MeshGenerator()
    {
        VerticesPerLine = SpecifiedChunkSizes[(FlatShadingActive) ? FlatshadedChunkSizeIndex : ChunkSizeIndex] + 5;
        MeshWorldScale = (VerticesPerLine - 3) * MeshScale;
    }

    public MeshGenerationData GenerateMesh(float[,] HeightMap,int LODValue)
    {
        int SkipIncrement = (LODValue == 0) ? 1 : LODValue * 2;
        //int MeshVerticesPerLine=VerticesPerLine

        Vector2 TopLeftCord = new Vector2(-1, 1) * MeshWorldScale / 2;
        MeshGenerationData MeshGenData = new MeshGenerationData(VerticesPerLine, SkipIncrement, false);
        int[,] VertexMax = new int[VerticesPerLine, VerticesPerLine];

        MeshGenData = SetVertices(TopLeftCord, SkipIncrement, VertexMax, HeightMap, MeshGenData);


        return MeshGenData;
    }

    private MeshGenerationData SetVertices(Vector2 TopLeftCord, int SkipValue, int[,] VertexMapRef, float[,] HeightMapRef, MeshGenerationData MeshGenRef)
    {

        for (int x = 0; x < VerticesPerLine; x++)
        {
            for (int y = 0; y < VerticesPerLine; y++)
            {
                bool OutOfMeshVertex = y == 0 || y == VerticesPerLine - 1 || x == 0 || x == VerticesPerLine - 1;
                bool VertexSkipped = x > 2 && x < VerticesPerLine - 3 && y > 2 && y < VerticesPerLine - 3 && ((x - 2) % SkipValue != 0 || (y - 2) % SkipValue != 0);
                if (OutOfMeshVertex)
                {
                    VertexMapRef[x, y] = OutOfMeshIndex;
                    OutOfMeshIndex--;
                }
                else if (VertexSkipped)
                {
                    VertexMapRef[x, y] = VerticesIndex;
                    VerticesIndex++;
                }
            }
        }

        for (int x = 0; x < VerticesPerLine; x++)
        {
            for (int y = 0; y < VerticesPerLine; y++)
            {
                bool VertexSkipped = x > 2 && x < VerticesPerLine - 3 && y > 2 && y < VerticesPerLine - 3 && ((x - 2) % SkipValue != 0 || (y - 2) % SkipValue != 0);

                if (VertexSkipped)
                {
                    bool OutOfMeshVertex = y == 0 || y == VerticesPerLine - 1 || x == 0 || x == VerticesPerLine - 1;
                    bool AtMeshEdge = ((y == 1 || y == VerticesPerLine - 2 || x == 1 || x == VerticesPerLine - 2) && !OutOfMeshVertex);
                    bool IsMainVertex = (x - 2) % SkipValue == 0 && (y - 2) % SkipValue == 0 && !OutOfMeshVertex && !AtMeshEdge;
                    bool VertexEdgeConnected = (y == 2 || y == VerticesPerLine - 3 || x == 2 || x == VerticesPerLine - 3) && !OutOfMeshVertex && !AtMeshEdge && !IsMainVertex;

                    int VertexIndex = VertexMapRef[x, y];
                    Vector2 Percent = new Vector2(x - 1, y - 1) / (VerticesPerLine - 3);
                    Vector2 VertexPosition = TopLeftCord + new Vector2(Percent.x, Percent.y) * MeshWorldScale;

                    float Height = HeightMapRef[x, y];
                    if (VertexEdgeConnected)
                    {
                        bool VertexVertical = x == 2 || x == VerticesPerLine - 2;

                        int VertexA_Distance = ((VertexVertical) ? y - 2 : x - 2) % SkipValue;
                        int VertexB_Distance = (SkipValue - VertexA_Distance);

                        float PercentageDistanceFromA_To_B = VertexA_Distance / (float)SkipValue;
                        float VertexA_Height = HeightMapRef[(VertexVertical) ? x : x - VertexA_Distance, (VertexVertical) ? y - VertexA_Distance : y];
                        float VertexB_Height = HeightMapRef[(VertexVertical) ? x : x + VertexA_Distance, (VertexVertical) ? y + VertexA_Distance : y];

                        Height = VertexA_Height * (1 - PercentageDistanceFromA_To_B) + VertexB_Height * PercentageDistanceFromA_To_B;
                    }

                    MeshGenRef.AddVertex(new Vector3(VertexPosition.x, Height, VertexPosition.y), Percent, VertexIndex);

                    bool CreateTriangle = x < VerticesPerLine - 1 && y < VerticesPerLine- 1 && (!VertexEdgeConnected || (x != 2 && y != 2));

                    if (!CreateTriangle)
                    {
                        break;
                    }
                    
                    int CurrentIncriment = (IsMainVertex && x != VerticesPerLine - 3 && y != VerticesPerLine - 3) ? SkipValue : 1;
                    int VertexA = VertexMapRef[x, y];
                    int VertexB = VertexMapRef[x + CurrentIncriment, y];
                    int VertexC = VertexMapRef[x , y + CurrentIncriment];
                    int VertexD = VertexMapRef[x + CurrentIncriment, y + CurrentIncriment];

                    MeshGenRef.AddTriangle(VertexA, VertexD, VertexC);
                    MeshGenRef.AddTriangle(VertexD, VertexA, VertexB);

                }


            }
        }

        MeshGenRef.MeshProcessing();
        return MeshGenRef;
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