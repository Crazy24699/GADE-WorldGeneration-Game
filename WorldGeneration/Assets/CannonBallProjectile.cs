using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallProjectile : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(Lifetime());
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(10);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider Collider)
    {
        if (Collider.CompareTag("Enemy AI"))
        {
            //Collider.GetComponent<BaseEnemy>().HandleHealth(-10);
            Destroy(gameObject);
        }
        if (Collider.CompareTag("Ground") )
        {
            Destroy(gameObject);
        }
    }
}
