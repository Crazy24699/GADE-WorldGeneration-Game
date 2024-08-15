using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DefenderHall : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject TowerDefenderRef;
    public float HalfDist;
    public float MaxDefenderPlaceDist;
    [SerializeField] private List<GameObject> EnemySpawns = new List<GameObject>();

    [SerializeField] private int TowerSpawnNum = 12;

    public GameObject VertLocation;

    [SerializeField] private float DefenderPlacementRange;

    [Header("Vector"),Space(5)]
    [SerializeField]private Vector3 StartPosition;
    [SerializeField]private Vector3 EndPosition;
    [SerializeField]private Vector3 StartX;
    [SerializeField]private Vector3 EndX;


    public void DefenderStart()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            FindBarriers();
            //Instantiate(Coob, new Vector3(0, 3, -15), Quaternion.identity).transform.localScale = Scale;
            VisualizeSpacing();
        }
    }

    private void SpawnDefenderTowers()
    {

    }
    
    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(0.75f);
        Instantiate(TowerDefenderRef);
    }

    private void FindBarriers()
    {
        GameObject GroundObject = GameObject.FindGameObjectWithTag("Ground");
        MeshFilter GroundMeshFilterRef = GroundObject.GetComponent<MeshFilter>();

        Mesh GroundMesh = GroundMeshFilterRef.sharedMesh;


        Vector3 MaxPosition = GroundObject.transform.TransformPoint(GroundMesh.bounds.max);
        Vector3 MinPosition = GroundObject.transform.TransformPoint(GroundMesh.bounds.min);

        StartX = new Vector3(MaxPosition.x, 0, 0);
        EndX = new Vector3(MinPosition.x, 0, 0);
        StartPosition = new Vector3(0, 0, MaxPosition.z);
        EndPosition = new Vector3(0, 0, MinPosition.z);


        HalfDist = MinPosition.z - MaxPosition.z;

        Instantiate(VertLocation, new Vector3(0, 2, 1 * HalfDist / 2.75f), Quaternion.identity).transform.SetParent(this.gameObject.transform);
        GameObject StartPoint=Instantiate(VertLocation, new Vector3(0, 2, 1), Quaternion.identity);


        StartPoint.transform.SetParent(this.gameObject.transform);
        StartPoint.transform.localPosition = new Vector3(0, -0.5f, -1);

        HalfDist = StartPoint.transform.InverseTransformPoint(StartPoint.transform.localPosition).z;
        float HalfDistRounded = Mathf.Ceil(Mathf.Abs(HalfDist) / 5) * 5;
        HalfDist = (HalfDist < 0 ? HalfDistRounded * -1 : HalfDistRounded * 1)+10;
        Debug.Log(HalfDist);

    }

    private void VisualizeSpacing()
    {
        for (int i = 0; i < Mathf.Abs(HalfDist) /2; i++)
        {
            GameObject VisualPoint = Instantiate(VertLocation, new Vector3(0, 2, 1 * HalfDist + (i * 5)), Quaternion.identity);
            VisualPoint.transform.SetParent(this.transform);
            VisualPoint.transform.localPosition= new Vector3(0, 0, 1 * HalfDist + (i * 2)); 
            Debug.Log("kiss");
        }

        
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(1, 0, 0, 0.5f);
        //Gizmos.DrawCube(new Vector3(0,3,-15), Scale);
    }

}
