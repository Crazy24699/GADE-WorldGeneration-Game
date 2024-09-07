using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public GameObject UseablePoint;
    private MapGenerator MapGenScript;

    private void Start()
    {

        MapGenScript = FindObjectOfType<MapGenerator>();
        //RequestUseableLand();
        InvokeRepeating(nameof(RequestUseableLand), 0.0f, 0.5f);
    }

    public void RequestUseableLand()
    {
        if (!MapGenScript.MapGenerated)
        {
            return;
        }

        Vector3[] AllLand = MapGenScript.GeneratedMesh.GetComponent<MeshFilter>().mesh.vertices;
        Vector3 MinLandValue = AllLand.OrderBy(V => V.y).First();

        IEnumerable<Vector3> UseableLand = AllLand.Where(V => V.y <= MinLandValue.y+0.25);
        foreach (Vector3 Point in UseableLand)
        {
            //Instantiate(UseablePoint, Point, Quaternion.identity);
        }

        CancelInvoke();
    }


}