using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class PathGenerator : MonoBehaviour
{

    public GameObject PathFinderRef;

    public Vector3 MinVector;
    public Vector3 MaxVector;

    public Transform StartPoint;

    public MeshCollider MeshCol;
    public MeshFilter MeshFilterRef;

    public GameObject PathRef;

    public Vector3[] MeshVertices;
    public Vector3[] WorldVertices;

    public List<Transform> WaypointList;

    public int[] MeshTriangles;
    public int[] WorldTriangles;

    public LayerMask GroundLayer;


    private void Start()
    {
        WorldVertices = MeshFilterRef.sharedMesh.vertices;
        WorldTriangles = MeshFilterRef.sharedMesh.triangles;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            HandleFinding();
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            MeshCol.sharedMesh = MeshFilterRef.sharedMesh;
            StartCoroutine(FindPath(StartPoint));
        }
    }

    public void HandleFinding()
    {

        MeshVertices = PathFinderRef.GetComponent<MeshFilter>().mesh.vertices;
        MeshTriangles = PathFinderRef.GetComponent<MeshFilter>().mesh.triangles;

        for (int i = 0; i < MeshVertices.Length; i++)
        {
            MeshVertices[i] = PathFinderRef.transform.TransformPoint(PathFinderRef.GetComponent<MeshFilter>().mesh.vertices[i]);
            Debug.Log(PathFinderRef.GetComponent<MeshFilter>().sharedMesh.bounds.max);
        }

        foreach (var Vertex in MeshVertices)
        {



            if (MinVector == Vector3.zero)
            {
                MinVector = Vertex;
            }
            if (Vertex.x < MinVector.x)
            {
                MinVector=Vertex;
            }

            if(MaxVector == Vector3.zero)
            {
                MaxVector = Vertex;
            }

            if(Vertex.x > MaxVector.x)
            {
                MaxVector=Vertex;
            }

        }

        Vector3.Min(MinVector, MaxVector);

        float Distance = MinVector.x - MaxVector.x;

        Instantiate(new GameObject(), MaxVector, Quaternion.identity);
        Instantiate(new GameObject(), MinVector, Quaternion.identity);

        StartCoroutine(Intervals(Mathf.Abs(Distance)));

    }

    private IEnumerator Intervals(float Distance)
    {
        Vector3 PathPosition = MinVector;

        yield return new WaitForSeconds(0.52f);



        for (int i = 0; i < Distance / 8; i++)
        {
            yield return new WaitForSeconds(0.2f);
            RaycastHit HitObject;

            bool Hit = Physics.Raycast(PathPosition, Vector3.down, out HitObject, 100.0f, GroundLayer);
            Debug.Log(HitObject.collider.gameObject.name);

            Debug.Log(HitObject.triangleIndex);

            if (Hit)
            {
                int FirstVertexCord = WorldTriangles[HitObject.triangleIndex * 3 + 0];
                int SecondVertexCord = WorldTriangles[HitObject.triangleIndex * 3 + 1];
                int ThirdVertexCord = WorldTriangles[HitObject.triangleIndex * 3 + 2];
                int ForthVertexCord = WorldTriangles[HitObject.triangleIndex * 3 + 3];

                WorldVertices[FirstVertexCord] += Vector3.up;
                WorldVertices[SecondVertexCord] += Vector3.up;
                WorldVertices[ThirdVertexCord] += Vector3.up;

                WorldVertices[ForthVertexCord] += Vector3.up;

                Vector3 FaceCord = (WorldVertices[FirstVertexCord] + WorldVertices[SecondVertexCord] 
                    + WorldVertices[ThirdVertexCord] + WorldVertices[ForthVertexCord]) / 4;

                Vector3 PathCord = WorldVertices[FirstVertexCord];

                Debug.Log(PathCord);

                Instantiate(PathRef, FaceCord+=Vector3.up*2, Quaternion.identity);
                PathPosition.x += 10;

                
            }

        }
        MeshFilterRef.mesh.vertices=WorldVertices;
    }


    private IEnumerator FindPath(Transform StartingPoint)
    {

        float UpperXRange = StartingPoint.position.x + 200;
        float LowerXRange = StartingPoint.position.x - 150;

        float Upper_Z_Range = StartingPoint.position.z + 20;
        float Lower_Z_Range = StartingPoint.position.z - 20;
        
        float Lower_YRange = StartingPoint.position.y - 7.75f;
        float Upper_YRange = StartingPoint.position.y + 5.75f;



        IEnumerable<Vector3> VectorsInRange=WorldVertices.Where(
            MatchingVectors=>MatchingVectors.y >= Lower_YRange&& MatchingVectors.y<=Upper_YRange 
            &&MatchingVectors.x>=LowerXRange && MatchingVectors.x<=UpperXRange 
            && MatchingVectors.z >= Lower_Z_Range && MatchingVectors.z <= Upper_Z_Range);
        List<Vector3> UseableVectors = VectorsInRange.ToList();

        Debug.Log(VectorsInRange.Count());
        float UpdatedXPos= transform.position.x;
        for (int i = 0; i < 4; i++)
        {
            UpdatedXPos += 40;
            List<Vector3> PossibleVectors = VectorsInRange.Where(Vectors => Vectors.x >= UpdatedXPos-4.5f
            && Vectors.x<=UpdatedXPos+4.5f).ToList();

            Vector3 RandomWaypoint = new Vector3(UpdatedXPos, 0, 0);
            Debug.Log(PossibleVectors.Count);
            if(PossibleVectors.Count > 0)
            {
                int WaypointIndex=Random.Range(0,PossibleVectors.Count);
                Instantiate(StartingPoint, PossibleVectors[WaypointIndex], Quaternion.identity);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

}


class SplineMeshClass
{
    public MeshRenderer Renderer;
    public MeshFilter Filter;
    public Mesh meshRef;
    public Transform[] ControlPoints;  // Array of control points for the spline
    public int Resolution = 10;  // Number of segments between each control point
    public float Width = 1.0f;  // Width of the mesh

    private void Start()
    {
        if (ControlPoints.Length < 2) return;

        Mesh mesh = CreateMeshFromSpline(ControlPoints, Resolution, Width);
        //GetComponent<MeshFilter>().mesh = mesh;
    }

    private Mesh CreateMeshFromSpline(Transform[] controlPoints, int resolution, float width)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3[] splinePoints = SampleSpline(controlPoints, resolution);

        for (int i = 0; i < splinePoints.Length; i++)
        {
            Vector3 forward = Vector3.zero;

            if (i < splinePoints.Length - 1)
            {
                forward = (splinePoints[i + 1] - splinePoints[i]).normalized;
            }
            else
            {
                forward = (splinePoints[i] - splinePoints[i - 1]).normalized;
            }

            Vector3 perpendicular = Vector3.Cross(forward, Vector3.up).normalized;

            Vector3 vertex1 = splinePoints[i] - perpendicular * width * 0.5f;
            Vector3 vertex2 = splinePoints[i] + perpendicular * width * 0.5f;

            vertices.Add(vertex1);
            vertices.Add(vertex2);

            if (i < splinePoints.Length - 1)
            {
                int startIndex = i * 2;

                triangles.Add(startIndex);
                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + 2);

                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + 3);
                triangles.Add(startIndex + 2);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        // Optionally recalculate normals and UVs
        mesh.RecalculateNormals();

        return mesh;
    }

    private Vector3[] SampleSpline(Transform[] controlPoints, int resolution)
    {
        List<Vector3> sampledPoints = new List<Vector3>();

        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            Vector3 p0 = controlPoints[Mathf.Max(i - 1, 0)].position;
            Vector3 p1 = controlPoints[i].position;
            Vector3 p2 = controlPoints[i + 1].position;
            Vector3 p3 = controlPoints[Mathf.Min(i + 2, controlPoints.Length - 1)].position;

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 pointOnSpline = CatmullRomSpline(p0, p1, p2, p3, t);
                sampledPoints.Add(pointOnSpline);
            }
        }

        sampledPoints.Add(controlPoints[controlPoints.Length - 1].position); // Add last point
        return sampledPoints.ToArray();
    }

    private Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // Catmull-Rom spline calculation
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * (t * t) +
            (-p0 + 3f * p1 - 3f * p2 + p3) * (t * t * t)
        );
    }
}