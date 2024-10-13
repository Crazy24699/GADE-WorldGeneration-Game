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
    public float CurrentRange;

    public Transform Mortar;
    public Rigidbody ProjectilePrefab;
    public Transform FirePoint;

    public float MortarAngle = -20f; // Fixed shooting angle
    public float ProjectileMass = 1f; // Mass of the projectile
    public float ExtraForce = 1.25f; // Additional force factor for heavy projectiles

    private void Start()
    {
        // Ensure mass is set from the prefab
        ProjectileMass = ProjectilePrefab.mass;
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
        // Ensure a minimum distance to avoid unrealistic cases
        if (horizontalDistance < 0.1f)
        {
            return MinLaunchSpeed;
        }

        // Convert mortar angle to radians
        float angleInRadians = Mathf.Deg2Rad * Mathf.Abs(MortarAngle);
        float gravity = Mathf.Abs(Physics.gravity.y);

        // Simplified formula to calculate launch speed without drag (accounting for projectile mass and gravity)
        float speedSquared = (horizontalDistance * gravity) / Mathf.Sin(2 * angleInRadians);

        if (speedSquared <= 0f || float.IsNaN(speedSquared))
        {
            return MinLaunchSpeed;
        }

        float idealLaunchSpeed = Mathf.Sqrt(speedSquared);

        // Adjust the launch speed based on extra force (for heavier projectiles)
        float adjustedLaunchSpeed = idealLaunchSpeed * ExtraForce;

        // Clamp the launch speed between minimum and maximum values
        return Mathf.Clamp(adjustedLaunchSpeed, MinLaunchSpeed, MaxLaunchSpeed);
    }

    void FireProjectile(float launchSpeed)
    {
        if (ProjectilePrefab != null && FirePoint != null)
        {
            Rigidbody projectileInstance = Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);

            // Calculate the launch direction with a fixed angle
            Vector3 launchDirection = Quaternion.AngleAxis(MortarAngle, FirePoint.forward) * FirePoint.forward;

            // Set the velocity to fire the projectile
            projectileInstance.velocity = launchDirection * launchSpeed;

            // Apply mass to the projectile
            projectileInstance.mass = ProjectileMass;
        }
    }
}
