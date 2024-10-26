using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public GameObject UseablePoint;
    [SerializeField] private GameObject ChosenTower;
    [SerializeField] private Defenders ChosenTowerInfo;
    public GameObject Outliner;

    private MapGenerator MapGenScript;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Camera ViewingCamera;
    private int SpawnedTowerCount;

    private int Money;

    private bool TowerSelected = false;

    [SerializeField]private List<GameObject> DefenderTowers= new List<GameObject>();
    [SerializeField]private List<Defenders> DefenderOptions= new List<Defenders>();
    


    [SerializeField]private float MaxPlacementHeight;

    public TextMeshProUGUI PlacedTowersText;
    public TextMeshProUGUI MoneyCount;

    [SerializeField]private HandleOverlap CheckTowerClearance;

    private void Start()
    {
        ViewingCamera = Camera.main;
        MapGenScript = FindObjectOfType<MapGenerator>();
        CheckTowerClearance = FindObjectOfType<HandleOverlap>();
        //RequestUseableLand();
        InvokeRepeating(nameof(RequestUseableLand), 0.0f, 0.5f);
        ProgramManager.ProgramManagerInstance.SpawnWave.AddListener(() => AddMoreMoney());
        Money = 120;
    }

    private void AddMoreMoney()
    {
        if (ProgramManager.ProgramManagerInstance.WaveNum == 0)
        {
            ProgramManager.ProgramManagerInstance.WaveNum = 1;
        }
        Money = Money * ProgramManager.ProgramManagerInstance.WaveNum;
    }

    private void RequestUseableLand()
    {
        GameObject GroundParent = new GameObject("Useable Area Parent");
        GroundParent.transform.position = Vector3.zero;
        if (!MapGenScript.MapGenerated)
        {
            return;
        }

        Vector3[] AllLand = MapGenScript.GeneratedMesh.GetComponent<MeshFilter>().mesh.vertices;
        if(AllLand.Length <= 0)
        {
            return;
        }
        Vector3 MinLandValue = AllLand.OrderBy(V => V.y).First();

        IEnumerable<Vector3> UseableLand = AllLand.Where(V => V.y <= MinLandValue.y+0.35f);
        foreach (Vector3 Point in UseableLand)
        {
            Instantiate(UseablePoint, Point + Vector3.up * 1.5f, Quaternion.Euler(90, 0, 0)).transform.parent = GroundParent.transform;
        }

        MaxPlacementHeight = Mathf.Abs(MinLandValue.y) + 0.35f;
        CancelInvoke();
    }

    public void HandleMoney(int MoneyDifference)
    {
        Money += MoneyDifference;

    }

    public void SelectTower(string Name)
    {
        //ChosenTower = null;
        //ChosenTowerInfo = null;
        Defenders ChosenDefender = DefenderOptions.FirstOrDefault(Twr => Twr.Name == Name);
        int TowerCost = ChosenDefender.Cost;

        if (Money < TowerCost) 
        {
            return;
        }

        ChosenTower = ChosenDefender.TowerObject;
        ChosenTowerInfo = ChosenDefender;
        TowerSelected = true;
    }

    private void Update()
    {
        HandleTower();
        HandleUI();
    }

    private void HandleUI()
    {
        PlacedTowersText.text = string.Format($"Money Left {Money} /10");
    }

    private void HandleTower()
    {
        if(!TowerSelected)
        {
            return;
        }
        Ray MousePositionRay = ViewingCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit CurrentObject;

        if(Physics.Raycast(MousePositionRay, out CurrentObject,1000,GroundLayer))
        {
            Debug.Log(CurrentObject.collider.name+"     "+LayerMask.LayerToName(CurrentObject.collider.gameObject.layer));
            Outliner.transform.position = CurrentObject.point;
        }

        if (!CheckTowerClearance.AreaClear)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {

            Ray MouseRay = ViewingCamera.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit HitInfo;
            if (Physics.Raycast(MouseRay, out HitInfo, 25000.0f, GroundLayer)) 
            {

                if (Mathf.Abs(HitInfo.point.y) > MaxPlacementHeight  ) 
                {
                    Debug.Log("Too hight        "+HitInfo.point.y+3+"       "+MaxPlacementHeight);
                    return;
                }
                if(HitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    GameObject SpawnedTower = Instantiate(ChosenTower, HitInfo.point + Vector3.up * 10, Quaternion.identity);
                    SpawnedTower.gameObject.name = "Defender Tower " + SpawnedTowerCount + ChosenTower.gameObject.name;
                    Debug.Log(HitInfo.collider.name + HitInfo.collider.gameObject.layer);
                    DefenderTowers.Add(SpawnedTower.transform.GetComponentInChildren<TowerBase>().gameObject);
                    HandleMoney(-ChosenTowerInfo.Cost);

                    TowerSelected = false;
                }

            }
            
        }

    }

    [System.Serializable]
    private class Defenders
    {
        [SerializeField]public string Name;
        [SerializeField] public int Cost;
        [SerializeField] public GameObject TowerObject;
    }

}
