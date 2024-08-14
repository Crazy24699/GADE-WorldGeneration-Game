using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPTVertsFind : MonoBehaviour
{
    public Vector3 boxCenter = Vector3.zero;
    public Vector3 boxSize = Vector3.one;
    public Quaternion boxOrientation = Quaternion.identity;
    public Vector3 boxDirection = Vector3.forward;
    public float boxDistance = 5f;

    public List<Vector3> Verts;
    public GameObject Vert;

    void Start()
    {
        // Perform a BoxCast and collect hits
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter, boxSize / 2, boxDirection, boxOrientation, boxDistance);

        foreach (var hit in hits)
        {
            // Get the mesh filter of the hit object
            MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                // Get the world-space vertices within the BoxCast bounds
                Vector3[] validVertices = GetVerticesWithinBox(meshFilter, hit.transform);

                // Visualize or process the valid vertices
                foreach (var vertex in validVertices)
                {
                    Debug.DrawLine(vertex, vertex + Vector3.up * 0.1f, Color.red, 10f);
                    Debug.Log("Valid Vertex: " + vertex);
                    Instantiate(Vert, vertex, Quaternion.identity);
                }
            }
        }
    }

    Vector3[] GetVerticesWithinBox(MeshFilter meshFilter, Transform hitTransform)
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        List<Vector3> validVertices = new List<Vector3>();

        // Convert the half-extents and center to world space
        Vector3 halfExtents = boxSize / 2;
        Vector3 boxWorldCenter = hitTransform.InverseTransformPoint(boxCenter);
        Quaternion invRotation = Quaternion.Inverse(boxOrientation);

        foreach (var vertex in vertices)
        {
            // Transform the vertex to world space
            Vector3 worldVertex = hitTransform.TransformPoint(vertex);

            // Transform the vertex to the box's local space
            Vector3 localVertex = invRotation * (worldVertex - boxWorldCenter);

            // Check if the vertex is within the bounds of the box
            if (Mathf.Abs(localVertex.x) <= halfExtents.x &&
                Mathf.Abs(localVertex.y) <= halfExtents.y &&
                Mathf.Abs(localVertex.z) <= halfExtents.z)
            {
                validVertices.Add(worldVertex);
            }
        }

        return validVertices.ToArray();
    }
}
