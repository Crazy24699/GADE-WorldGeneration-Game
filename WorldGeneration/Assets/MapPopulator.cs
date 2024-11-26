using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapPopulator : MonoBehaviour
{
    public List<Vector3> TreePoints = new List<Vector3>();
    private MapGenerator MapGenScript;


    public Vector3 Center = Vector3.zero; // Center of the usable space
    public Vector3 Size = new Vector3(5, 1, 5); // Dimensions of the usable space
    public Color GizmoColor = Color.green; // Color of the gizmo
    public bool DrawOnSelectedOnly = false; // Draw the gizmo only when the object is selected

    [Space(2.5f), Header("Own Code")]

    public List<Vector3> Bounds = new List<Vector3>();
    public List<Vector3> AvaliablePoints;

    public Vector2 CurrentPosition;


    public LayerMask InvalidArea;
    public float CheckingRadius;
    public float HeightPoint;
    public GameObject TreePrefab;

    private bool CheckRan = false;

    // Start is called before the first frame update
    void Start()
    {
        MapGenScript = FindObjectOfType<MapGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TreePoints = RequestUnusableLand();

        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(Populator());
        }
    }
    void OnDrawGizmos()
    {
        DrawGizmo();
    }
    public List<Vector3> RequestUnusableLand()
    {

        if (!MapGenScript.MapGenerated)
        {
            return null;
        }

        Vector3[] AllLand = MapGenScript.GeneratedMesh.GetComponent<MeshFilter>().mesh.vertices;


        HeightPoint = AllLand.Max(V => V.y);

        IEnumerable<Vector3> UseableLand = AllLand.Where(V => V.y >= HeightPoint - 2.15f && V.y <= HeightPoint);

        //MaxPlacementHeight = Mathf.Abs(MaxTreeElevation) + 0.35f;
        UseableLand = new HashSet<Vector3>(UseableLand);
        return UseableLand.ToList();
    }
    private IEnumerator Populator()
    {
        if (CheckRan) { yield break; }
        CheckRan = true;
        for (int i = 0; i < 25; i++)
        {

            yield return new WaitForSeconds(0.525f);
            AreaChecker();
        }
    }

    private Vector3[] GetBoundingVertices()
    {
        // Define the local-space offsets for a cube's 8 vertices
        Vector3 HalfSize = Size * 0.5f;
        Vector3[] LocalOffsets = new Vector3[]
        {
            new Vector3(-HalfSize.x,  HalfSize.y, -HalfSize.z), // Top-back-left
            new Vector3( HalfSize.x,  HalfSize.y, -HalfSize.z), // Top-back-right
            new Vector3(-HalfSize.x,  HalfSize.y,  HalfSize.z), // Top-front-left
            new Vector3( HalfSize.x,  HalfSize.y,  HalfSize.z), // Top-front-right
        };

        // Transform local vertices to world space
        Vector3[] WorldVertices = new Vector3[LocalOffsets.Length];
        for (int i = 0; i < LocalOffsets.Length; i++)
        {
            WorldVertices[i] = transform.TransformPoint(Center + LocalOffsets[i]);
        }

        return WorldVertices;
    }

    private void DrawGizmo()
    {
        if(TreePoints.Count <= 0) { return; }
        /*
         Gizmos.color = Color.yellow;
         Gizmos.DrawSphere(new Vector3(CurrentPosition.x,3.25f, CurrentPosition.y), 1.5f);
         foreach (var Vertex in TreePoints)
         {
             Gizmos.color = Color.grey;
             Gizmos.DrawSphere(Vertex, 2);
         }
         */
    }

    private void AreaChecker()
    {

        Debug.Log("Hells bells");

        Bounds = GetBoundingVertices().ToList();

        //HashSet<float> XCollection = new HashSet<float>(TreePoints.Select(Vert => Vert.x));
        //HashSet<float> ZCollection = new HashSet<float>(TreePoints.Select(Vert => Vert.z));

        //List<float> XPosOptions = new List<float>(XCollection);
        //List<float> ZPosOptions = new List<float>(ZCollection);

        //float XPos = Random.Range(XPosOptions[0], XPosOptions[XPosOptions.Count - 1]);
        //float ZPos = Random.Range(ZPosOptions[0], ZPosOptions[ZPosOptions.Count - 1]);
        int RandomCord = Random.Range(0, TreePoints.Count);

        Vector2 PossiblePosition = new Vector2(TreePoints[RandomCord].x, TreePoints[RandomCord].z);

        Debug.Log(PossiblePosition);

        Collider[] DetectedColliders = Physics.OverlapSphere(new Vector3(TreePoints[RandomCord].x, 3.75f, TreePoints[RandomCord].z), CheckingRadius*2, InvalidArea);
        CurrentPosition = PossiblePosition;

        Debug.Log(DetectedColliders.Count()+"       nothing ");
        if (DetectedColliders.Length != 0)
        { Debug.Log("My church"); return; }
        PlaceTree(PossiblePosition);
    }

    private void PlaceTree(Vector2 TreePosition)
    {

        Instantiate(TreePrefab, new Vector3(TreePosition.x, 9.75f, TreePosition.y), Quaternion.identity);

    }


}
