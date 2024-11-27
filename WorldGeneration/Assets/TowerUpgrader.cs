using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgrader : MonoBehaviour
{
    public TowerUpgradeFunctionality SelectedTower;
    public LayerMask DefenderLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SelectTower();
        }
    }

    private void SelectTower()
    {
        Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit RayHit;

        if(Physics.Raycast(MouseRay, out RayHit, 250, DefenderLayer))
        {
            Debug.Log(RayHit.transform.gameObject.name);
            if (SelectedTower != null && SelectedTower != RayHit.transform.gameObject.GetComponentInChildren<TowerUpgradeFunctionality>())  
            {
                Debug.Log("got your words written on my skin");
                SelectedTower.DeselectTower();
            }
            SelectedTower = RayHit.transform.gameObject.GetComponentInChildren<TowerUpgradeFunctionality>();
            SelectedTower.SelectTower();
        }


    }

}
