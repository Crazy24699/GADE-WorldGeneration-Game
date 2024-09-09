using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleOverlap : MonoBehaviour
{
    public float Distance;
    public Vector3 PathDirection;
    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Pathway"))
        {
            Distance = Vector2.Distance(Collision.gameObject.transform.position, transform.position);
            Vector3 PathPos=Collision.gameObject.transform.position;
            PathDirection = PathPos;
        }
    }
}
