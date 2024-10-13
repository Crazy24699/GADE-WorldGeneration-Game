using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallProjectile : MonoBehaviour
{
    public int Damage;

    private float ReductionTime = 0.45f;
    private float CurrentTime;

    public bool ApplyMoreGravity = false;

    private bool CanReduce;


    private void Start()
    {
        StartCoroutine(Lifetime());
        //StartCoroutine(ReduceCooldown());
        //CurrentTime = ReductionTime;
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(10);
        Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        if(ApplyMoreGravity && CanReduce)
        {
            ExtraGravity();
        }
    }

    private void ExtraGravity()
    {
        if (CurrentTime > 0)
        {
            CurrentTime -= Time.fixedDeltaTime;
            if(CurrentTime <= 0)
            {
                CurrentTime = ReductionTime;
                Vector3 VelocityRef = GetComponent<Rigidbody>().velocity;
                GetComponent<Rigidbody>().velocity = new Vector3(VelocityRef.x, VelocityRef.y - 2.5f, VelocityRef.z);
                Debug.Log("She drove a thousand miles");
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
