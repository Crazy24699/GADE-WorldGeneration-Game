using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{

    public GameObject PathFinderRef;

    public Vector3 MinVector;
    public Vector3 MaxVector;

    public GameObject PathRef;

    public Vector3[] Vertices;

    public LayerMask GroundLayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            HandleFinding();
        }
    }


    public void HandleFinding()
    {

        Vertices = PathFinderRef.GetComponent<MeshFilter>().mesh.vertices;

        for (int i = 0; i < Vertices.Length; i++)
        {
            Vertices[i] = PathFinderRef.transform.TransformPoint(PathFinderRef.GetComponent<MeshFilter>().mesh.vertices[i]);
            Debug.Log(PathFinderRef.GetComponent<MeshFilter>().sharedMesh.bounds.max);
        }

        foreach (var Vertex in Vertices)
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
        
        for (int i = 0;i < Distance/10;i++)
        {
            yield return new WaitForSeconds(0.52f);
            RaycastHit Ray;
            if (Physics.Raycast(PathPosition, Vector3.down, out Ray, 10.0f, GroundLayer)) 
            {

            }
            Vector3 PathCord=Ray.point;

            Instantiate(PathRef, PathCord, Quaternion.identity);
            PathPosition.x += 10;
        }

    }

}
