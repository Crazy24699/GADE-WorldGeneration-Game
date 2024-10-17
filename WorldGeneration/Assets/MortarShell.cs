using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarShell : MonoBehaviour
{
    public Transform FirePoint;      
    public Transform TargetPoint;    
    public Vector3 EndPosition;
    public float TravelTime = 2;    
    [SerializeField]private float ElapsedTime = 0f; 
    private int Damage=50;

    private Vector3 ArcMidPoint;     

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
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime < TravelTime)
        {
            Debug.Log("Slerp");

            Vector3 StartToMidPoint = Vector3.Slerp(FirePoint.position, ArcMidPoint, ElapsedTime / TravelTime);
            Vector3 MidPointToEndPoint = Vector3.Slerp(ArcMidPoint, EndPosition, ElapsedTime / TravelTime);

            transform.position = Vector3.Slerp(StartToMidPoint, MidPointToEndPoint, ElapsedTime / TravelTime);
        }
    }
    private void OnTriggerEnter(Collider Collider)
    {
        if (Collider.CompareTag("Enemy AI"))
        {
            Collider.GetComponent<BaseEnemy>().HandleHealth(-Damage);
            Destroy(gameObject);
        }
        if (Collider.CompareTag("Ground") || Collider.CompareTag("Pathway"))
        {
            Destroy(gameObject);
        }
    }
}
