using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public GameObject UseablePoint;
    [SerializeField] private GameObject DefenderTower;

    private MapGenerator MapGenScript;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Camera ViewingCamera;
    private int SpawnedTowerCount;

    private bool TowerSelected = false;


    [SerializeField]private float MaxPlacementHeight;

    private void Start()
    {
        ViewingCamera = Camera.main;
        MapGenScript = FindObjectOfType<MapGenerator>();
        //RequestUseableLand();
        InvokeRepeating(nameof(RequestUseableLand), 0.0f, 0.5f);
    }

    private void RequestUseableLand()
    {
        if (!MapGenScript.MapGenerated)
        {
            return;
        }

        Vector3[] AllLand = MapGenScript.GeneratedMesh.GetComponent<MeshFilter>().mesh.vertices;
        Vector3 MinLandValue = AllLand.OrderBy(V => V.y).First();

        IEnumerable<Vector3> UseableLand = AllLand.Where(V => V.y <= MinLandValue.y+0.35f);
        foreach (Vector3 Point in UseableLand)
        {
            Instantiate(UseablePoint, Point+Vector3.up*1.5f, Quaternion.Euler(90,0,0));
        }

        MaxPlacementHeight = Mathf.Abs(MinLandValue.y) + 0.35f;
        CancelInvoke();
    }

    public void SelectTower()
    {
        TowerSelected = true;
    }

    private void Update()
    {
        HandleTower();
        
    }

    private void HandleTower()
    {
        if(!TowerSelected)
        {
            return;
        }

        
        if(Input.GetMouseButtonDown(0))
        {
            Ray MouseRay = ViewingCamera.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit HitInfo;
            if (Physics.Raycast(MouseRay, out HitInfo, 25000.0f, GroundLayer)) 
            {
                
                if(Mathf.Abs(HitInfo.point.y) >MaxPlacementHeight)
                {
                    Debug.Log("Too hight        "+HitInfo.point.y+3+"       "+MaxPlacementHeight);
                    return;
                }
                GameObject SpawnedTower = Instantiate(DefenderTower, HitInfo.point + Vector3.up * 10, Quaternion.identity);
                SpawnedTower.gameObject.name = "Defender Tower " + SpawnedTowerCount;
                Debug.Log(HitInfo.collider.name + HitInfo.collider.gameObject.layer);
                TowerSelected = false;
            }
            
        }

    }

}