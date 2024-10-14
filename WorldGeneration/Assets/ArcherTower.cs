using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : MonoBehaviour
{
    public Transform ObjectToRotate; // The object that will be rotated
    public Transform Target;          // The object to face
    public float RotationSpeed = 2f;  // Speed at which the object rotates
    public int RotationOffset;

    private void Update()
    {
        // Calculate the direction to the target
        Vector3 directionToTarget = Target.position - ObjectToRotate.position;
        directionToTarget.y = 0; // Keep the turret rotation on the Y-axis only


        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        targetRotation *= Quaternion.Euler(0, RotationOffset, 0);
        // Smoothly rotate towards the target
        ObjectToRotate.rotation = Quaternion.Slerp(ObjectToRotate.rotation, targetRotation, Time.deltaTime * RotationSpeed);    

        if (directionToTarget.magnitude > 0.1f) // Check if the target is not too close
        {
            
        }
    }


}
