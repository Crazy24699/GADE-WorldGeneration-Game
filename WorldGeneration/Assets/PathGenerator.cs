using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;


public class PathGenerator : MonoBehaviour
{

    public GameObject PathFinderRef;


    public Transform StartPoint;
    public Transform EndPoint;

    public MeshCollider MeshCol;
    public MeshFilter MeshFilterRef;

    public GameObject PathRef;
    public GameObject MadePath;
    public GameObject Visualiser;
    public GameObject SplineVisual;

    private EnemySpawnerLogic SpawnerRef;

    public Vector3 MinVector;
    public Vector3 MaxVector;

    public Vector3 ChangeValue;
    private List<Vector3> MeshVertices = new List<Vector3>();
    private List<Vector3> WorldVertices = new List<Vector3>();
    
    public List<Vector3> EnemyWaypoints;
    public List<Vector3> WaypointList = new List<Vector3>();

    [SerializeField]private Vector3 CastingOffset;
    [SerializeField] private Vector3 CastingSize;

    public int[] MeshTriangles;
    public int[] WorldTriangles;
    public int PathResolution;
    public int Resolution = 20;

    public bool Generated = false;

    public Transform[] ControlPointTransforms;

    public LayerMask GroundLayer;

    public float Width;
    [SerializeField]private float CastingDistance;
    public float HeightDiff;

    public Material MaterialRef;


    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            FindBottom();
        }
        if(!Generated) { return; }

        if (Input.GetKeyDown(KeyCode.G))
        {
            HandleGeneration();
        }
    }

    private void FindBottom()
    {
        Vector3[] NewCords = MeshFilterRef.mesh.vertices;
        for (int i = 0; i < NewCords.Length; i++)
        {
            Ray Ray = new Ray(NewCords[i], Vector3.down);  // Create a downward ray
            RaycastHit HitInfo;

            if (Physics.Raycast(Ray, out HitInfo, 20, GroundLayer))
            {
                
                NewCords[i] = new Vector3(HitInfo.point.x, HitInfo.point.y+HeightDiff, HitInfo.point.z);
                Debug.Log("Hit an object on the target layer: AAAAAAAAAAAAAAAAAAAAAAAS" + HitInfo.collider.gameObject.name);
                //Instantiate(Visualiser, HitInfo.point, Quaternion.identity);
                // Perform any logic when a collision with the specified layer occurs
            }

        }
        MeshFilterRef.mesh.vertices = NewCords;
        MeshFilterRef.mesh.RecalculateNormals();
    }

    public void HandleGeneration()
    {
        CreatePathObject();
        SpawnerRef = FindObjectOfType<EnemySpawnerLogic>();

        MeshCol.sharedMesh = MeshFilterRef.sharedMesh;
        StartCoroutine(MakePath(transform));

        Vector3[] controlPoints = ControlPointTransforms.Select(t => t.position).ToArray();
        MeshFilterRef.mesh = CreateMeshFromSpline(controlPoints, Resolution, Width);
        MeshCol.sharedMesh = MeshFilterRef.sharedMesh;
    }

    private void CreatePathObject()
    {
        //adds the first waypoint as the start of the path
        WaypointList.Add(transform.position);

        MadePath = new GameObject("Path" + this.gameObject.name);
        MadePath.transform.position = Vector3.zero;
        MadePath.transform.parent = transform;

        //adds components to create and discplay the mesh
        MadePath.AddComponent<MeshCollider>();
        MadePath.AddComponent<MeshRenderer>();
        MadePath.GetComponent<MeshRenderer>().material = MaterialRef;
        MadePath.AddComponent<MeshFilter>();

        MapGenerator MapGenRef = FindObjectOfType<MapGenerator>();
        MeshFilterRef = MadePath.GetComponent<MeshFilter>();
        MeshCol = MadePath.GetComponent<MeshCollider>();

        MeshFilter Terrain = MapGenRef.GeneratedMesh;

        WorldVertices = Terrain.sharedMesh.vertices.ToList();
        WorldTriangles = Terrain.sharedMesh.triangles;
    }

    private IEnumerator MakePath(Transform StartingPoint)
    {
        yield return new WaitForSeconds(0.25f);

        List<Vector3> Points = new List<Vector3>();
        Points.Add(StartingPoint.position);
        Points.Add(StartingPoint.position + Vector3.right * 10);

        float UpperXRange = StartingPoint.position.x + 220;
        float LowerXRange = StartingPoint.position.x - 220;

        float Upper_Z_Range = StartingPoint.position.z + 20;
        float Lower_Z_Range = StartingPoint.position.z - 20;

        float Lower_YRange = StartingPoint.position.y - 7.75f;
        float Upper_YRange = StartingPoint.position.y + 5.75f;

        IEnumerable<Vector3> VectorsInRange = WorldVertices.Where(
            MatchingVectors => MatchingVectors.y >= Lower_YRange && MatchingVectors.y <= Upper_YRange &&
            MatchingVectors.x >= LowerXRange && MatchingVectors.x <= UpperXRange
            && MatchingVectors.z >= Lower_Z_Range && MatchingVectors.z <= Upper_Z_Range);
        List<Vector3> UseableVectors = VectorsInRange.ToList();

        //Debug.Log(VectorsInRange.Count() + "      " + WorldVertices.Count());

        foreach (var Point in VectorsInRange)
        {
            //Instantiate(Visualiser, Point, Quaternion.identity);
        }
        //MeshVertices.AddRange(VectorsInRange.ToList());

        NavMeshSurface NavSurface = MadePath.AddComponent<NavMeshSurface>();
        MadePath.layer = 8;

        NavSurface.layerMask = SceneHandler.SceneInstance.WalkableLayers;

        MadePath.tag = "Pathway";
        NavSurface.BuildNavMesh();

        SceneHandler.SceneInstance.PathsGenerated = true;
    }

    private Mesh CreateMeshFromSpline(Vector3[] WorldPoints, int SplineResolution, float Width)
    {
        List<Vector3> SplineVertices = new List<Vector3>();
        List<int> SplineTriangles = new List<int>();

        Vector3[] SplinePoints = SampleSpline(WorldPoints, SplineResolution);

        for (int i = 0; i < SplinePoints.Length; i++)
        {
            Vector3 GenerationDirection = Vector3.zero;

            if (i < SplinePoints.Length - 1)
            {
                GenerationDirection = (SplinePoints[i + 1] - SplinePoints[i]).normalized;
            }
            else
            {
                GenerationDirection = (SplinePoints[i] - SplinePoints[i - 1]).normalized;
            }

            Vector3 PathWidth = Vector3.Cross(GenerationDirection, Vector3.up).normalized;

            Vector3 Vertex1 = SplinePoints[i] - PathWidth * Width * 0.5f;
            Vector3 Vertex2 = SplinePoints[i] + PathWidth * Width * 0.5f;

            SplineVertices.Add(Vertex1);
            SplineVertices.Add(Vertex2);

            if (i < SplinePoints.Length - 1)
            {
                int StartingIndex = i * 2;

                SplineTriangles.Add(StartingIndex);
                SplineTriangles.Add(StartingIndex + 1);
                SplineTriangles.Add(StartingIndex + 2);

                SplineTriangles.Add(StartingIndex + 1);
                SplineTriangles.Add(StartingIndex + 3);
                SplineTriangles.Add(StartingIndex + 2);
            }
        }

        Mesh MeshRef = new Mesh();
        MeshRef.vertices = SplineVertices.ToArray();
        MeshRef.triangles = SplineTriangles.ToArray();
        MeshRef.RecalculateNormals();

        return MeshRef;
    }

    private Vector3[] SampleSpline(Vector3[] WorldPoints, int SplineResolution)
    {
        List<Vector3> SplinePoints = new List<Vector3>();

        for (int i = 0; i < WorldPoints.Length - 1; i++)
        {
            //Creates a mesh based off the spline points, each spline point having a min and max vector point
            //translated from world values 
            Vector3 Point0 = WorldPoints[Mathf.Max(i - 1, 0)];
            Vector3 Point1 = WorldPoints[i];
            Vector3 Point2 = WorldPoints[i + 1];
            Vector3 Point3 = WorldPoints[Mathf.Min(i + 2, WorldPoints.Length - 1)];

            for (int j = 0; j < SplineResolution; j++)
            {
                float t = j / (float)SplineResolution;
                Vector3 ExtraSplinePoints = CatmullRomSpline(Point0, Point1, Point2, Point3, t);
                SplinePoints.Add(ExtraSplinePoints);
            }
        }

        SplinePoints.Add(WorldPoints[WorldPoints.Length - 1]);
        return SplinePoints.ToArray();
    }

    private Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            (2f * p1) + (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * (t * t) +
            (-p0 + 3f * p1 - 3f * p2 + p3) * (t * t * t)
        );
    }

}
