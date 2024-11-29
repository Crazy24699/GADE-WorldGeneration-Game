using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarDefender : TowerBase
{
    public float MaxRange = 100f;
    public float FiringAngle = -20f;
    public float GravityMultiplier = 2f;
    private float NormalGravity;

    public float FireRate = 0.95f;


    [SerializeField] private float ShotCooldownTime = 0f;

    public GameObject ProjectileRef;
    [SerializeField] private List<GameObject> TargetList = new List<GameObject>();
    
    public Transform FirePoint;

    [SerializeField] private LayerMask EnemyLayer;

    [SerializeField] private Material DamageMat;
    [SerializeField] private Material NormalMat;



    private bool CanShoot = true;


    void Start()
    {
        NormalGravity = Physics.gravity.y * GravityMultiplier;
        MaxHealth = 50;
        TowerStartup();
        NormalMat = TowerMeshRender.materials[0];
        Damage = 50;
    }

    private void Update()
    {

        for (int i = 0; i < TargetList.Count; i++)
        {
            //Debug.Log(TargetList[i].name);
            if (TargetList[i] == null)
            {
                TargetList.RemoveAt(i);
            }
        }
        AttackTargets();
        TrackTarget();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireProjectile();
        }
    }

    private void AttackTargets()
    {
        if (TargetList.Count > 0)
        {
            ShotCooldownTime += Time.deltaTime;

            // Check for player input and shoot if the cooldown has elapsed
            if (ShotCooldownTime >= FireRate)
            {
                Debug.Log("Dont remembver");
                ShotCooldownTime = 0; // Reset cooldown timer
                FireProjectile();
            }

        }
    }

    public void FireProjectile()
    {
        float TargetDistance = Vector3.Distance(FirePoint.position, TargetList[0].transform.position);

        if (TargetDistance > MaxRange)
        {
            Debug.LogWarning("Target is out of maximum range.");
            return;
        }

        float AppliedForce = CalculateFiringForce(TargetDistance);

        GameObject MortarShell = Instantiate(ProjectileRef, FirePoint.position, Quaternion.identity);
        MortarShell.GetComponent<MortarShell>().Damage = Damage;
        MortarShell.GetComponent<MortarShell>().ShellStart(FirePoint, TargetList[0].transform);
    }

    private void TrackTarget()
    {
        if (!StartupRan) { return; }
        if(TargetList.Count <= 0) { return; }
        Vector3 TargetRotation = TargetList[0].transform.position - transform.position;
        TargetRotation.y = 0;
        transform.rotation = Quaternion.LookRotation(TargetRotation);
    }

    private float CalculateFiringForce(float TargetDistance)
    {
        float IntialShellVelocity = 50f; 
        float Angle = Mathf.Deg2Rad * FiringAngle;

        // Calculate firing force using projectile motion formulas
        float AppliedFireForce = (TargetDistance * NormalGravity) / (Mathf.Sin(2 * Angle) * IntialShellVelocity);
        return AppliedFireForce;
    }
    private void OnTriggerEnter(Collider Collision)
    {
        if (!StartupRan) { return; }
        if (Collision.CompareTag("Enemy AI"))
        {
            if (!TargetList.Contains(Collision.gameObject))
            {
                TargetList.Add(Collision.gameObject);
            }
            Debug.Log("HA");
        }
    }

}
