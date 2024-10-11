using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class MeshEditTst : MonoBehaviour
{
    private Mesh Mesh;
    private List<Vector3> Vertices;
    private List<int> Triangles;
    private List<Vector2> Uvs;

    public Vector3[] AddedVertices;

    public List<VertexEdit> VertexInfo=new List<VertexEdit>();

    void Start()
    {
        // Initialize the mesh and its components
        Mesh = GetComponent<MeshFilter>().mesh;

        // Initialize the vertex, triangle, and UV lists
        Vertices = new List<Vector3>(Mesh.vertices);
        Triangles = new List<int>(Mesh.triangles);
        Uvs = new List<Vector2>(Mesh.uv);

        // Create a simple initial mesh (a triangle as an example)
        foreach (var Vertex in Mesh.vertices)
        {
            VertexInfo.Add(new VertexEdit());
            VertexInfo[VertexInfo.Count-1].Vertex = Vertex;
            VertexInfo[VertexInfo.Count-1].Index = VertexInfo.Count;

        }
        //AddInitialVertices();
        UpdateMesh();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Adds a new vertex at a random position within a certain range
            //Vector3 randomVertex = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            HashSet<Vector3> NewVertices = new HashSet<Vector3>(Vertices);
            int Counter = 0;
            foreach (var Vertex in AddedVertices)
            {
                
                if (!NewVertices.Contains(Vertex))
                {
                    AddVertex(Vertex);
                }

                Counter++;
            }
            UpdateMesh();
            
        }
    }

    // This adds a vertex to the mesh dynamically
    public void AddVertex(Vector3 newVertex, bool createTriangle = true)
    {
        // Add the new vertex to the list
        Vertices.Add(newVertex);

        // Create a UV for the new vertex
        Uvs.Add(new Vector2(newVertex.x, newVertex.y)); // Simplified UV mapping for now

        // Optionally, create a triangle using the last three vertices (if enough vertices exist)
        if (Vertices.Count >= 3 && createTriangle)
        {
            int count = Vertices.Count;
            Triangles.Add(count - 3);
            Triangles.Add(count - 2);
            Triangles.Add(count - 1);
        }

        // Update the mesh after adding the vertex
        UpdateMesh();
    }

    // Updates the mesh with the current vertices, triangles, and uvs
    private void UpdateMesh()
    {
        Mesh.Clear();

        // Apply the new data to the mesh

        for (int i = 0; i < VertexInfo.Count; i++)
        {
            Vertices[i] = VertexInfo[i].Vertex;
        }

        Mesh.SetVertices(Vertices);
        Mesh.SetTriangles(Triangles, 0);
        Mesh.SetUVs(0, Uvs);

        // Recalculate normals and bounds for proper shading and rendering
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
        Debug.Log("Snowflake");
    }

    // Adds initial vertices to form a basic triangle
    private void AddInitialVertices()
    {
        // Define an initial triangle
        Vertices.Add(new Vector3(0, 0, 0)); // Vertex 0
        Vertices.Add(new Vector3(1, 0, 0)); // Vertex 1
        Vertices.Add(new Vector3(0, 1, 0)); // Vertex 2

        // Define UVs (for basic texture mapping)
        Uvs.Add(new Vector2(0, 0));
        Uvs.Add(new Vector2(1, 0));
        Uvs.Add(new Vector2(0, 1));

        // Define triangles using the indices of the vertices
        Triangles.Add(0);
        Triangles.Add(1);
        Triangles.Add(2);
    }

}

[System.Serializable]
public class VertexEdit
{
    public Vector3 Vertex;
    public int Index;
}
