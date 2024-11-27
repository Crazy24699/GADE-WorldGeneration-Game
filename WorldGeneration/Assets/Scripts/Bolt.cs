using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    [SerializeField] private float LaunchForce = 30;
    [SerializeField] public int Damage = 30;
    private Rigidbody RigidbodyRef;

    public void Startup(Vector3 Direction)
    {

        RigidbodyRef = GetComponent<Rigidbody>();

        RigidbodyRef.AddForce(-transform.right * LaunchForce, ForceMode.Impulse);
    }


    private void OnTriggerEnter(Collider Collider)
    {
        if (Collider.CompareTag("Enemy AI"))
        {
            Collider.GetComponent<BaseEnemy>().HandleHealth(-Damage);
            Destroy(gameObject);
        }
        if (Collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
