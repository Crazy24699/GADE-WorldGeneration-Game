using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TreePopulator : MonoBehaviour
{
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


    private void Start()
    {
        CurrentPosition = Vector2.zero ;
    }

    void OnDrawGizmos()
    {
        if (!DrawOnSelectedOnly)
        {
            DrawGizmo();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (DrawOnSelectedOnly)
        {
            DrawGizmo();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(Populator());
        }
    }

    private void FindOpenPoints()
    {

    }

    private IEnumerator Populator()
    {
        if(CheckRan) { yield break; }
        CheckRan = true;
        for (int i = 0; i < 15; i++)
        {

            yield return new WaitForSeconds(0.25f);
            AreaChecker();
        }
    }


    private void DrawGizmo()
    {
        Gizmos.color = GizmoColor;

        Vector3 worldCenter = transform.TransformPoint(Center);
        Vector3 worldSize = Vector3.Scale(Size, transform.lossyScale);

        Gizmos.DrawWireCube(worldCenter, worldSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(worldCenter, CheckingRadius);


        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z));

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

    

    private void AreaChecker()
    {
        
        Debug.Log("Hells bells");

        Bounds = GetBoundingVertices().ToList();

        HashSet<float> XCollection = new HashSet<float>(Bounds.Select(Vert => Vert.x));
        HashSet<float> ZCollection = new HashSet<float>(Bounds.Select(Vert => Vert.z));

        List<float> XPosOptions = new List<float>(XCollection);
        List<float> ZPosOptions = new List<float>(ZCollection);

        float XPos = Random.Range(XPosOptions[0], XPosOptions[XPosOptions.Count-1]);
        float ZPos = Random.Range(ZPosOptions[0], ZPosOptions[ZPosOptions.Count-1]);

        Vector2 PossiblePosition = new Vector2(XPos, ZPos);

        Debug.Log(PossiblePosition);

        Collider[] DetectedColliders = Physics.OverlapSphere(new Vector3(XPos, 7.75f, ZPos), CheckingRadius, InvalidArea);

        Debug.Log(DetectedColliders.Count());
        if (DetectedColliders.Length != 0)
        {Debug.Log("My church"); return; }
        PlaceTree(PossiblePosition);
    }

    private void PlaceTree(Vector2 TreePosition)
    {

        Instantiate(TreePrefab, new Vector3(TreePosition.x, 7.75f, TreePosition.y), Quaternion.identity);

    }


}
