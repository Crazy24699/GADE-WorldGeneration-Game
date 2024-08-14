using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DefenderHall : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject TowerDefenderRef;
    public float HalfDist;
    [SerializeField] private List<GameObject> EnemySpawns = new List<GameObject>();

    [SerializeField] private int TowerSpawnNum = 12;

    public GameObject VertLocation;

    [SerializeField] private float DefenderPlacementRange;

    public Vector3 Scale;

    public void DefenderStart()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            FindBarriers();
            //Instantiate(Coob, new Vector3(0, 3, -15), Quaternion.identity).transform.localScale = Scale;
            
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
        MeshRenderer GroundMeshRendRef = GroundObject.GetComponent<MeshRenderer>();
        MeshFilter GroundMeshFilterRef = GroundObject.GetComponent<MeshFilter>();

        Mesh GroundMesh = GroundMeshFilterRef.sharedMesh;
        foreach (var Vertex in GroundMesh.vertices)
        {

            //Debug.Log(Vertex);
        }
        Vector3 MaxPosition = GroundObject.transform.TransformPoint(GroundMesh.bounds.max);
        Vector3 MinPosition = GroundObject.transform.TransformPoint(GroundMesh.bounds.min);

        Instantiate(VertLocation, MaxPosition, Quaternion.identity);
        Instantiate(VertLocation, MinPosition, Quaternion.identity);

        HalfDist = MinPosition.z - MaxPosition.z;

        Instantiate(VertLocation, new Vector3(0, 2, 1*HalfDist / 2.75f), Quaternion.identity).transform.SetParent(this.gameObject.transform);
        GameObject StartPoint=Instantiate(VertLocation, new Vector3(0, 2, 1), Quaternion.identity);
        StartPoint.transform.SetParent(this.gameObject.transform);
        StartPoint.transform.localPosition = new Vector3(0, -0.5f, -1);


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(0,3,-15), Scale);
    }

}
