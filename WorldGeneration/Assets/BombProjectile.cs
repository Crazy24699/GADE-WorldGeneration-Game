using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    public int Damage;

    private void Start()
    {
        StartCoroutine(Lifetime());
    }

    private void OnTriggerEnter(Collider Collider)
    {
        if (Collider.CompareTag("TowerArea") || Collider.CompareTag("DefenderTower")) 
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

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(10);
        Destroy(this.gameObject);
    }

}
