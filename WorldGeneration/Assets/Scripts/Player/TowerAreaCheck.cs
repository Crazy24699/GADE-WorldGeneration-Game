using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAreaCheck : MonoBehaviour
{
    private TowerLogic TowerLogicParent;

    private void Start()
    {
        TowerLogicParent = transform.GetComponentInParent<TowerLogic>();
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.gameObject.CompareTag("PlayerTower") || Collision.gameObject.CompareTag("Pathway"))
        {
            //TowerLogicParent.DetermineAreaClearance(false);
            return;
        }
        //TowerLogicParent.DetermineAreaClearance(true);
    }

}
