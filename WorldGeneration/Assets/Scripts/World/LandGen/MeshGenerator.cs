using UnityEngine;
using System.Collections;

public class MeshGenerator
{
    private AnimationCurve HeightCurveValue;
    private int MeshIncrimentValue = 0;
    public GameObject This;

    public MeshGenerationData GenerateTerrainMesh(float[,] HeightMap, float HeightMultiplier, AnimationCurve HeightCurveTemplate, int LevelOfDetail)
    {
        HeightCurveValue = new AnimationCurve(HeightCurveTemplate.keys);
        //Debug.Log(HeightMap[5, 5] + "      " + MapGenerator.Current.name);

        MeshIncrimentValue = (LevelOfDetail == 0) ? 1 : LevelOfDetail * 2;

        Debug.Log(HeightMap.Length);

        int BorderedSize = HeightMap.GetLength(0);
        int MeshSize = BorderedSize - 2 * MeshIncrimentValue;
        int MeshSizeUnsimplified = BorderedSize - 2;

        float TopLeftX = (MeshSizeUnsimplified - 1) / -2f;
        float TopLeftZ = (MeshSizeUnsimplified - 1) / 2f;

        int VerticesPerLine = (MeshSize - 1) / MeshIncrimentValue + 1;

        MeshGenerationData MeshData = new MeshGenerationData(VerticesPerLine);

        int[,] VertexIndicesMap = new int[BorderedSize, BorderedSize];
        int MeshVertexIndex = 0;
        int BorderVertexIndex = -1;
        for (int x = 0; x < BorderedSize; x += MeshIncrimentValue)
        {
            for (int y = 0; y < BorderedSize; y += MeshIncrimentValue)
            {
                bool IsBorderVertex = y == 0 || y == BorderedSize - 1 || x == 0 || x == BorderedSize - 1;

                if (IsBorderVertex)
                {
                    VertexIndicesMap[x, y] = BorderVertexIndex;
                    BorderVertexIndex--;
                }
                else
                {
                    VertexIndicesMap[x, y] = MeshVertexIndex;
                    MeshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < BorderedSize; y += MeshIncrimentValue)
        {
            for (int x = 0; x < BorderedSize; x += MeshIncrimentValue)
            {
                int VertexIndex = VertexIndicesMap[x, y];
                Vector2 Percent = new Vector2((x - MeshIncrimentValue) / (float)MeshSize, (y - MeshIncrimentValue) / (float)MeshSize);
                float Height = HeightCurveValue.Evaluate(HeightMap[x, y]) * HeightMultiplier;
                Vector3 VertexPosition = new Vector3(TopLeftX + Percent.x * MeshSizeUnsimplified, Height, TopLeftZ - Percent.y * MeshSizeUnsimplified);

                MeshData.AddVertex(VertexPosition, Percent, VertexIndex);

                if (x < BorderedSize - 1 && y < BorderedSize - 1)
                {
                    int PointA = VertexIndicesMap[x, y];
                    int PointB = VertexIndicesMap[x + MeshIncrimentValue, y];
                    int PointC = VertexIndicesMap[x, y + MeshIncrimentValue];
                    int PointD = VertexIndicesMap[x + MeshIncrimentValue, y + MeshIncrimentValue];

                    MeshData.AddTriangle(PointA, PointD, PointC);
                    MeshData.AddTriangle(PointD, PointA, PointB);
                }

                VertexIndex++;
            }
        }

        MeshData.BakeNormals();

        return MeshData;

    }
}

public class MeshGenerationData
{


    public Vector3[] Vertices;
    private int[] Triangles;
    private Vector2[] UVs;
    private Vector3[] BakedNormals;

    public int VertsPerLine;

    public Vector3[] BorderVertices;
    public int[] BorderTriangles;

    int TriangleIndex;
    public int BorderTriangleIndex;

    public MeshGenerationData(int VerticesPerLine)
    {
        Vertices = new Vector3[VerticesPerLine * VerticesPerLine];

        UVs = new Vector2[VerticesPerLine * VerticesPerLine];
        Triangles = new int[(VerticesPerLine - 1) * (VerticesPerLine - 1) * 6];

        VertsPerLine = VerticesPerLine;

        BorderVertices = new Vector3[VerticesPerLine * 4 + 4];
        BorderTriangles = new int[24 * VerticesPerLine];



    }

    public void AddVertex(Vector3 VertexPosition, Vector2 Uv, int VertexIndex)
    {
        if (VertexIndex < 0)
        {
            BorderVertices[-VertexIndex - 1] = VertexPosition;
        }
        else
        {
            Vertices[VertexIndex] = VertexPosition;
            UVs[VertexIndex] = Uv;
        }
    }

    public void AddTriangle(int A, int B, int C)
    {
        if (A < 0 || B < 0 || C < 0)
        {
            BorderTriangles[BorderTriangleIndex] = A;
            BorderTriangles[BorderTriangleIndex + 1] = B;
            BorderTriangles[BorderTriangleIndex + 2] = C;
            BorderTriangleIndex += 3;
        }
        else
        {
            Triangles[TriangleIndex] = A;
            Triangles[TriangleIndex + 1] = B;
            Triangles[TriangleIndex + 2] = C;
            TriangleIndex += 3;
        }
    }

    Vector3[] CalculateNormals()
    {
        Vector3[] VertexNormals = new Vector3[Vertices.Length];
        int TriangleCount = Triangles.Length / 3;
        for (int i = 0; i < TriangleCount; i++)
        {
            int NormalTriangleIndex = i * 3;
            int VertexIndexA = Triangles[NormalTriangleIndex];
            int VertexIndexB = Triangles[NormalTriangleIndex + 1];
            int VertexIndexC = Triangles[NormalTriangleIndex + 2];

            Vector3 TriangleNormal = SurfaceNormalFromIndices(VertexIndexA, VertexIndexB, VertexIndexC);
            VertexNormals[VertexIndexA] += TriangleNormal;
            VertexNormals[VertexIndexB] += TriangleNormal;
            VertexNormals[VertexIndexC] += TriangleNormal;
        }

        int BorderTriangleCount = BorderTriangles.Length / 3;
        for (int I = 0; I < BorderTriangleCount; I++)
        {
            int NormalTriangleIndex = I * 3;
            int VertexIndexA = BorderTriangles[NormalTriangleIndex];
            int VertexIndexB = BorderTriangles[NormalTriangleIndex + 1];
            int VertexIndexC = BorderTriangles[NormalTriangleIndex + 2];

            Vector3 TriangleNormal = SurfaceNormalFromIndices(VertexIndexA, VertexIndexB, VertexIndexC);
            if (VertexIndexA >= 0)
            {
                VertexNormals[VertexIndexA] += TriangleNormal;
            }
            if (VertexIndexB >= 0)
            {
                VertexNormals[VertexIndexB] += TriangleNormal;
            }
            if (VertexIndexC >= 0)
            {
                VertexNormals[VertexIndexC] += TriangleNormal;
            }
        }

        for (int I = 0; I < VertexNormals.Length; I++)
        {
            VertexNormals[I].Normalize();
        }

        return VertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int IndexA, int IndexB, int IndexC)
    {
        Vector3 PointA = (IndexA < 0) ? BorderVertices[-IndexA - 1] : Vertices[IndexA];
        Vector3 PointB = (IndexB < 0) ? BorderVertices[-IndexB - 1] : Vertices[IndexB];
        Vector3 PointC = (IndexC < 0) ? BorderVertices[-IndexC - 1] : Vertices[IndexC];

        Vector3 SideAB = PointB - PointA;
        Vector3 SideAC = PointC - PointA;
        return Vector3.Cross(SideAB, SideAC).normalized;
    }

    public void BakeNormals()
    {
        BakedNormals = CalculateNormals();
    }

    public Mesh CreateMesh()
    {
        Mesh MeshRef = new Mesh();

        MeshRef.vertices = Vertices;
        MeshRef.triangles = Triangles;
        MeshRef.uv = UVs;
        MeshRef.normals = BakedNormals;

        Debug.Log("Frgot to laugh");

        foreach (var Vet in Vertices)
        {
            //Debug.Log(Vet);
        }

        return MeshRef;
    }
}