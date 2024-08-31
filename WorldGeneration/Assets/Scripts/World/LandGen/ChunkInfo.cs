using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ChunkInfo : MonoBehaviour
{
    public MeshGenerationData Meshdata;

    public Vector3[] BorderVertices;
    [HideInInspector]public Vector3[] AllVertices;
    [HideInInspector]public int[] BorderTriangles;

    public bool Populated = false;


    public void HandleMeshInfo()
    {
        BorderVertices = Meshdata.BorderVertices;
        BorderTriangles = Meshdata.BorderTriangles;

        AllVertices = Meshdata.Vertices;

        //Change this later

        for (int i = 0; i < AllVertices.Length; i++)
        {
            Vector3 CurrentCoordinate = Meshdata.Vertices[i];
            Vector3 UpdatedCoordinate = new Vector3(transform.localScale.x * CurrentCoordinate.x,
                transform.localScale.y * CurrentCoordinate.y, transform.localScale.z * CurrentCoordinate.z);

            UpdatedCoordinate += transform.position;
            AllVertices[i] = UpdatedCoordinate;
        }

        for (int i = 0; i < Meshdata.BorderVertices.Length; i++)
        {
            Vector3 CurrentCoordinate = Meshdata.BorderVertices[i];
            Vector3 UpdatedCoordinate = new Vector3(transform.localScale.x * CurrentCoordinate.x,
                transform.localScale.y * CurrentCoordinate.y, transform.localScale.z * CurrentCoordinate.z);

            UpdatedCoordinate += transform.position;
            BorderVertices[i] = UpdatedCoordinate;
        }

        Populated = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //EditMesh();
            BorderVertices=Meshdata.PopulateBorder().ToArray();
            Debug.Log("Mind"+ BorderVertices.Length);
            BorderVertices = BorderVertices.ToArray();
        }

        if (Meshdata != null)
        {
            //Debug.Log("you are to defy me, NO ONE WINS MY GAME");
        }

    }

    private void OnDrawGizmos()
    {
        if (!Populated)
        {
            return;
        }

        //foreach (var Cord in BorderVertices)
        //{
        //    Gizmos.color = Color.cyan;

        //    //Vector3 Result = new Vector3();
        //    //Result += transform.position;

        //    Gizmos.DrawSphere(Cord, .5f);
        //    //Debug.Log("true");
        //}
    }

}
