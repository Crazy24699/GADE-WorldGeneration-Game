using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PathGenerator : MonoBehaviour
{

    public GameObject PathFinderRef;

    public Vector3 MinVector;
    public Vector3 MaxVector;

    public Vector3 ChangeValue;

    public Transform StartPoint;
    public Transform EndPoint;

    public MeshCollider MeshCol;
    public MeshFilter MeshFilterRef;

    public GameObject PathRef;
    public GameObject MadePath;
    public GameObject Visualiser;

    private EnemySpawnerLogic SpawnerRef;

    public Vector3[] MeshVertices;
    public Vector3[] WorldVertices;

    public List<Vector3> WaypointList=new List<Vector3>();

    public List<Vector3> EnemyWaypoints;

    public int[] MeshTriangles;
    public int[] WorldTriangles;

    public LayerMask GroundLayer;

    public int PathResolution;
    public float Width;

    public Material MaterialRef;

    private void Start()
    {
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //HandleFinding();
        }

        if(Input.GetKeyDown(KeyCode.G))
        {

            HandleGeneration();
        }
    }

    public void HandleGeneration()
    {
        WorldVertices = MeshFilterRef.sharedMesh.vertices;
        WorldTriangles = MeshFilterRef.sharedMesh.triangles;

        //MeshFilterRef.AddComponent<MeshFilter>();

        Debug.Log(WaypointList.Count);

        WaypointList.Add(transform.position);
        MadePath = new GameObject("Path" + this.gameObject.name);
        MadePath.transform.position = Vector3.zero;
        MadePath.transform.parent = transform;

        MadePath.AddComponent<MeshCollider>();
        MadePath.AddComponent<MeshRenderer>();
        MadePath.GetComponent<MeshRenderer>().material = MaterialRef;
        MadePath.AddComponent<MeshFilter>();



        SpawnerRef =FindObjectOfType<EnemySpawnerLogic>();

        MeshCol.sharedMesh = MeshFilterRef.sharedMesh;
        StartCoroutine(FindPath(StartPoint));

        Debug.Log("Little wolf"+this.gameObject);

    }


  

    private IEnumerator FindPath(Transform StartingPoint)
    {

        float UpperXRange = StartingPoint.position.x + 220;
        float LowerXRange = StartingPoint.position.x - 220;

        float Upper_Z_Range = StartingPoint.position.z + 20;
        float Lower_Z_Range = StartingPoint.position.z - 20;
        
        float Lower_YRange = StartingPoint.position.y - 7.75f;
        float Upper_YRange = StartingPoint.position.y + 5.75f;



        IEnumerable<Vector3> VectorsInRange=WorldVertices.Where(
            MatchingVectors=>MatchingVectors.y >= Lower_YRange&& MatchingVectors.y<=Upper_YRange 
            &&MatchingVectors.x>=LowerXRange && MatchingVectors.x<=UpperXRange 
            && MatchingVectors.z >= Lower_Z_Range && MatchingVectors.z <= Upper_Z_Range);
        List<Vector3> UseableVectors = VectorsInRange.ToList();

        //Debug.Log(VectorsInRange.Count());
        float UpdatedXPos= transform.position.x;
        for (int i = 0; i < 4; i++)
        {
            UpdatedXPos += 40;
            List<Vector3> PossibleVectors = VectorsInRange.Where(Vectors => Vectors.x >= UpdatedXPos-4.5f
            && Vectors.x<=UpdatedXPos+4.5f).ToList();

            Vector3 RandomWaypoint = new Vector3(UpdatedXPos, 0, 0);
            //Debug.Log(PossibleVectors.Count);
            if(PossibleVectors.Count > 0)
            {
                int WaypointIndex=Random.Range(0,PossibleVectors.Count);
                if (PossibleVectors[WaypointIndex] != Vector3.zero)
                {
                    WaypointList.Add(PossibleVectors[WaypointIndex]);
                    //Debug.Log(PossibleVectors[WaypointIndex]);
                }
                else if(PossibleVectors[WaypointIndex] == Vector3.zero)
                {
                    //Debug.Log("we go");
                    WaypointList.Add(PossibleVectors[WaypointIndex-3]+new Vector3(2.5f,2.5f,0));
                    //Debug.Log(WaypointList[WaypointList.Count]);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
        WaypointList.Add(EndPoint.transform.position + ChangeValue * 3);

        MadePath.transform.position += Vector3.up * 2;
        Mesh MeshRef = CreateMeshPath(WaypointList, PathResolution, Width);
        //StartCoroutine(Points(MeshRef));
        MeshRef = ConformPathToMesh(MeshRef);

        MeshRef=VerifyVectors(MeshRef);
        MadePath.GetComponent<MeshFilter>().mesh = MeshRef;
        //MadePath.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        //SetEnemyWaypoints(MeshRef.vertices);
        NavMeshSurface NavSurface = MadePath.AddComponent<NavMeshSurface>();
        MadePath.layer = 8;
        //Debug.Log(NavMesh.GetAreaFromName("Ground") + "        nobody");
        NavSurface.layerMask = SceneHandler.SceneInstance.WalkableLayers;

        MadePath.tag = "Pathway";
        NavSurface.BuildNavMesh();

        //ProgramManager.ProgramManagerInstance.SpawnWave.Invoke();

        SpawnerRef.CheckSpawnList(this);


        //Debug.Log("Fight");
    }

    private Mesh VerifyVectors(Mesh MeshRef)
    {
        Vector3[] Vertices = MeshRef.vertices;
        for (int i = 0; i < Vertices.Length; i++)
        {
            if (Vertices[i] == Vector3.zero)
            {
                int AccessIndex = (i == 0) ? i : i-1;
                Vertices[i] = Vertices[AccessIndex] + ChangeValue * 3;
            }

        }
        MeshRef.vertices = Vertices;
        return MeshRef;
    }

    private Mesh ConformPathToMesh(Mesh CreatedMesh)
    {

        Vector3[] PathVectors = CreatedMesh.vertices;
        for (int i = 0; i < PathVectors.Length; i++)
        {

            Vector3 VertexChange = DetectConformDirection(Vector3.up, PathVectors[i]);
            if (VertexChange == Vector3.zero)
            {
                VertexChange = DetectConformDirection(Vector3.down, PathVectors[i]);
            }
            //Debug.Log(MadePath.name + " " + VertexChange+"      " + PathVectors[i]);
            PathVectors[i] = VertexChange;
        }

        CreatedMesh.vertices = PathVectors;
        return CreatedMesh;
    }

    public void SetEnemyWaypoints(Vector3[] PathVectors)
    {
        for (int i = 0; i < PathVectors.Length; i++)
        {
            if (i % 2 == 0 && i >= 2)
            {
                Vector3 WaypointVector;
                WaypointVector = PathVectors[i] + new Vector3(0, 3f, 1f);
                EnemyWaypoints.Add(WaypointVector);
                //Instantiate(Visualiser, WaypointVector, Quaternion.identity);
            }
        }
    }

    public Vector3 DetectConformDirection(Vector3 Direction, Vector3 RayStartingPosition)
    {

        RaycastHit HitObject;

        bool Hit = Physics.Raycast(RayStartingPosition+(new Vector3(0,3,0)), Direction, out HitObject, 1000.0f, GroundLayer);
        Vector3 MeshVertex = Vector3.zero ;

        if (HitObject.collider != null && HitObject.collider.gameObject.CompareTag("Ground")) 
        {
            MeshVertex = HitObject.point;
            //Debug.Log("Prevent");
        }

        return MeshVertex;
    }

    private Mesh CreateMeshPath(List<Vector3> controlPoints, int resolution, float width)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Sample points on the spline
        Vector3[] splinePoints = SampleSpline(controlPoints, resolution);

        for (int i = 0; i < splinePoints.Length; i++)
        {
            Vector3 forward;

            // Ensure smooth forward vector calculation
            if (i < splinePoints.Length - 1)
            {
                forward = (splinePoints[i + 1] - splinePoints[i]).normalized;
            }
            else
            {
                forward = (splinePoints[i] - splinePoints[i - 1]).normalized;
            }

            // Calculate perpendicular vector to create width
            Vector3 perpendicular = Vector3.Cross(forward, Vector3.up).normalized;

            Vector3 vertex1 = splinePoints[i] - perpendicular * width * 0.5f;
            Vector3 vertex2 = splinePoints[i] + perpendicular * width * 0.5f;

            // Add vertices for the current segment
            vertices.Add(vertex1);
            vertices.Add(vertex2);

            // Add triangles connecting vertices for each segment
            if (i < splinePoints.Length - 1)
            {
                int startIndex = i * 2;

                triangles.Add(startIndex);     // First triangle
                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + 2);

                triangles.Add(startIndex + 1); // Second triangle
                triangles.Add(startIndex + 3);
                triangles.Add(startIndex + 2);
            }
        }

        // Create and return the mesh
        Mesh pathMesh = new Mesh();
        pathMesh.vertices = vertices.ToArray();
        pathMesh.triangles = triangles.ToArray();

        // Ensure normals and bounds are recalculated
        pathMesh.RecalculateNormals();
        pathMesh.RecalculateBounds();

        return pathMesh;
    }

    // Spline interpolation (unchanged)
    private Vector3[] SampleSpline(List<Vector3> controlPoints, int resolution)
    {
        List<Vector3> sampledPoints = new List<Vector3>();

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            Vector3 p0 = controlPoints[Mathf.Max(i - 1, 0)];
            Vector3 p1 = controlPoints[i];
            Vector3 p2 = controlPoints[i + 1];
            Vector3 p3 = controlPoints[Mathf.Min(i + 2, controlPoints.Count - 1)];

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 pointOnSpline = CatmullRomSpline(p0, p1, p2, p3, t);
                sampledPoints.Add(pointOnSpline);
            }
        }

        sampledPoints.Add(controlPoints[controlPoints.Count - 1]);
        return sampledPoints.ToArray();
    }

    // Spline interpolation (unchanged)
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
