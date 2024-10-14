using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarDefender : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float maximumRange = 100f;
    public float angleOfAttack = -20f;
    public float gravityMultiplier = 2f;

    public Transform target;
    public Transform firePoint;

    private float gravity;

    void Start()
    {
        gravity = Physics.gravity.y * gravityMultiplier;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireProjectile();
        }
    }

    public void FireProjectile()
    {
        // Calculate distance to the target
        float distanceToTarget = Vector3.Distance(firePoint.position, target.position);

        // Ensure the target is within the maximum range
        if (distanceToTarget > maximumRange)
        {
            Debug.LogWarning("Target is out of maximum range.");
            return;
        }

        // Calculate firing force based on distance and angle
        float firingForce = CalculateFiringForce(distanceToTarget);

        // Create projectile and apply initial velocity
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<MortarShell>().ShellStart(firePoint, target);
    }

    private float CalculateFiringForce(float distance)
    {
        // Adjust parameters as needed based on your specific projectile and environment
        float initialVelocity = 50f; // Adjust initial velocity as necessary
        float angleRadians = Mathf.Deg2Rad * angleOfAttack;

        // Calculate firing force using projectile motion formulas
        float firingForce = (distance * gravity) / (Mathf.Sin(2 * angleRadians) * initialVelocity);
        return firingForce;
    }

}
