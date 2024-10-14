using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallProjectile : MonoBehaviour
{
    public int Damage;

    private float ReductionTime = 0.1f;
    private float CurrentTime;
    private float CurrentMass;

    public bool ApplyMoreGravity = false;

    private bool CanReduce;


    private void Start()
    {
        CurrentMass = gameObject.GetComponent<Rigidbody>().mass;
        StartCoroutine(Lifetime());
        StartCoroutine(ReduceCooldown());
        CurrentTime = ReductionTime;
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(10);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        ExtraGravity();
        if (ApplyMoreGravity && CanReduce)
        {
            
        }
    }

    private void ExtraGravity()
    {
        CurrentTime = ReductionTime;
        Vector3 VelocityRef = GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().AddForce(Vector3.down * 9.81f * 2, ForceMode.Force);
        Debug.Log("She drove a thousand miles");

        if (CurrentTime > 0)
        {
            CurrentTime -= Time.deltaTime;
            if(CurrentTime <= 0)
            {

            }
        }
    }

    private IEnumerator ReduceCooldown()
    {
        yield return new WaitForSeconds(0.75f);
        CanReduce = true;
    }

    private void OnTriggerEnter(Collider Collider)
    {
        if (Collider.CompareTag("Enemy AI"))
        {
            Collider.GetComponent<BaseEnemy>().HandleHealth(-Damage);
            Destroy(gameObject);
        }
        if (Collider.CompareTag("Ground") )
        {
            Destroy(gameObject);
        }
    }
}
