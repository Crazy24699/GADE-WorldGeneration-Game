using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DefenderTower : MonoBehaviour
{

    [SerializeField] private GameObject RotateObject;
    [SerializeField] private GameObject FirePoint;
    [SerializeField] private LayerMask EnemyLayer;

    public GameObject Projectile;

    private bool CanShoot = true;

    public float FireRate = 0.95f;  
    public int Damage = 10;  
    public float MaxShootDistace = 100f; 

    private float ShotCooldownTime = 0f; 


    [SerializeField]private List<GameObject> TargetList = new List<GameObject>();

    private void Start()
    {
        
    }

    private void Update()
    {

        Debug.DrawRay(FirePoint.transform.position, FirePoint.transform.forward*3, Color.blue);
        for (int i = 0; i < TargetList.Count; i++)
        {
            //Debug.Log(TargetList[i].name);
            if (TargetList[i]==null)
            {
                Debug.Log("fuck");
                TargetList.RemoveAt(i);
            }
        }
        AttackTargets();
    }

    private void AttackTargets()
    {
        if(TargetList.Count > 0)
        {
            RotateObject.transform.rotation = Quaternion.LookRotation(TargetList[0].transform.position-RotateObject.transform.position);

            ShotCooldownTime += Time.deltaTime;

            // Check for player input and shoot if the cooldown has elapsed
            if (ShotCooldownTime >= FireRate)
            {
                Debug.Log("aaaaa");
                FireCannon();
                ShotCooldownTime = 0f; // Reset cooldown timer
            }

        }
    }

    private void FireCannon()
    {
        GameObject ProjectileInstance = Instantiate(Projectile, FirePoint.transform.position, FirePoint.transform.rotation);

        Rigidbody ProjectileRB = ProjectileInstance.GetComponent<Rigidbody>();

        ProjectileRB.velocity = FirePoint.transform.forward * -250;
        TargetList[0].GetComponent<BaseEnemy>().HandleHealth(-10);
        if(TargetList[0].GetComponent<BaseEnemy>().CurrentHealth == 0)
        {
            TargetList.RemoveAt(0);
        }
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if(Collision.CompareTag("Enemy AI"))
        {
            if (!TargetList.Contains(Collision.gameObject))
            {
                TargetList.Add(Collision.gameObject);
            }
            Debug.Log("HA");
        }
    }
}
