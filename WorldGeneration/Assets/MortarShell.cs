using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarShell : MonoBehaviour
{
    public Transform FirePoint;      // The point where the projectile starts
    public Transform TargetPoint;    // The point the projectile is aiming for
    public Vector3 EndPosition;
    public float TravelTime = 0.25f;    // Time it takes to travel from fire point to target
    private float ElapsedTime = 0f;  // Tracks the time passed for interpolation
    private int Damage;

    private Vector3 ArcMidPoint;     // A calculated midpoint to simulate the arc

    private void Start()
    {

    }

    public void ShellStart(Transform FirePointRef,Transform TargetPointRef)
    {

        FirePoint = FirePointRef;
        TargetPoint = TargetPointRef;

        // Calculate a midpoint above the FirePoint and TargetPoint to create an arc
        EndPosition = TargetPoint.position;
        ArcMidPoint = (FirePoint.position + EndPosition) / 2f;
        ArcMidPoint += Vector3.up * Vector3.Distance(FirePoint.position, EndPosition) / 1.5f;
    }

    private void Update()
    {
        if (ElapsedTime < TravelTime)
        {
            // Increment the elapsed time
            ElapsedTime += Time.deltaTime;

            Debug.Log("Slerp");

            // Interpolate between the fire point and the target point using Slerp
            Vector3 startToMid = Vector3.Slerp(FirePoint.position, ArcMidPoint, ElapsedTime / TravelTime);
            Vector3 midToTarget = Vector3.Slerp(ArcMidPoint, EndPosition, ElapsedTime / TravelTime);

            // Set the shell's position to interpolate along the arc
            transform.position = Vector3.Slerp(startToMid, midToTarget, ElapsedTime / TravelTime);
        }
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
