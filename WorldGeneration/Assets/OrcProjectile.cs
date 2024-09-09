using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcProjectile : MonoBehaviour
{

    public int Damage;
    private void OnTriggerEnter(Collider Collider)
    {
        if (Collider.CompareTag("TowerArea"))
        {
            Debug.Log(Collider.transform.parent);
            Collider.transform.parent.GetComponentInChildren<DefenderTower>().HandleHealth(-Damage);
            Destroy(gameObject);
        }
        if (Collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
