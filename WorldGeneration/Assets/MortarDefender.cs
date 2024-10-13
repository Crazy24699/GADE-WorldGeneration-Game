using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarDefender : MonoBehaviour
{
    public Transform Target;
    public float MinLaunchSpeed = 10f;
    public float MaxLaunchSpeed = 30f;
    public float MaxRange = 50f;
    public float CurrentLaunchSpeed;
    public Transform Mortar;
    public Rigidbody ProjectilePrefab;
    public Transform FirePoint;

    public float MortarAngle = -20f; // Fixed shooting angle
    public float ProjectileMass = 1f; // Mass of the projectile
    public float ProjectileDrag = 0.1f; // Drag of the projectile


    private void Start()
    {
        ProjectileMass = ProjectilePrefab.mass;
        ProjectileDrag = ProjectilePrefab.drag;
    }

    private void Update()
    {
        if (Target != null)
        {
            Vector3 targetPosition = Target.position;
            Vector3 startPosition = Mortar.position;

            Vector3 directionToTarget = new Vector3(targetPosition.x - startPosition.x, 0, targetPosition.z - startPosition.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            Mortar.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            float horizontalDistance = Vector3.Distance(new Vector3(startPosition.x, 0, startPosition.z), new Vector3(targetPosition.x, 0, targetPosition.z));

            // Calculate launch speed based on distance and angle
            float launchSpeed = CalculateLaunchSpeed(horizontalDistance);
            CurrentLaunchSpeed = launchSpeed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                FireProjectile(launchSpeed);
            }
        }
    }

    float CalculateLaunchSpeed(float horizontalDistance)
    {
        if (horizontalDistance < 0.1f)
        {
            return MinLaunchSpeed;
        }

        float angleInRadians = Mathf.Deg2Rad * Mathf.Abs(MortarAngle);
        float gravity = Mathf.Abs(Physics.gravity.y);
        float sinOfAngle = Mathf.Sin(2 * angleInRadians);

        // Avoid division by zero
        if (Mathf.Abs(sinOfAngle) < 0.001f)
        {
            return MinLaunchSpeed;
        }

        // Calculate the ideal launch speed without drag
        float speedSquared = (horizontalDistance * gravity) / sinOfAngle;

        if (speedSquared <= 0f)
        {
            return MinLaunchSpeed;
        }

        float idealLaunchSpeed = Mathf.Sqrt(speedSquared);

        // Estimate the effect of drag
        float dragFactor = 1 + (ProjectileDrag * horizontalDistance / ProjectileMass);
        float adjustedLaunchSpeed = idealLaunchSpeed * dragFactor;

        // Clamp the launch speed between the minimum and maximum launch speeds
        return Mathf.Clamp(adjustedLaunchSpeed, MinLaunchSpeed, MaxLaunchSpeed);
    }

    void FireProjectile(float launchSpeed)
    {
        if (ProjectilePrefab != null && FirePoint != null)
        {
            Rigidbody projectileInstance = Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);

            // Set the firing angle to the fixed angle (-20 degrees) and apply velocity
            projectileInstance.velocity = Quaternion.AngleAxis(MortarAngle, FirePoint.forward) * FirePoint.forward * launchSpeed;

            // Set mass and drag for the projectile
            projectileInstance.mass = ProjectileMass;
            projectileInstance.drag = ProjectileDrag;
        }
    }
}
