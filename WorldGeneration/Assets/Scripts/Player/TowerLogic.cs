using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerLogic : TowerBase
{
    public Collider ColliderObject;

    public GameObject TowerObject;
    public GameObject TriggerCheck;

    public void DetermineAreaClearance(bool AreaClear)
    {
        switch (AreaClear)
        {
            case true:
                TowerStartup();
                break;
            
            case false:
                Destroy(this.gameObject);
                break;
        }
    }

    

    private void SpawnTower()
    {
        for (int i = 0; i < 10; i++)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            
        }
        //OnTriggerEnter(ColliderObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Collider>().name == "AreaOccupation" && other.transform.parent.name != this.gameObject.name)
        {
            Debug.Log("This works");
            return; 
        }
        else if(other.gameObject.GetComponent<Collider>().name == "AreaOccupation")
        {
            Debug.Log("this doesnt");
        }
    }
}
