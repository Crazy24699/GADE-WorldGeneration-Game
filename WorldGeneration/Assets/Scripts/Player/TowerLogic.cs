using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TowerLogic : TowerBase
{
    public PlayerUI PlayerUIScript;

    [SerializeField] private GameObject RotateObject;
    [SerializeField] private GameObject FirePoint;
    [SerializeField] private LayerMask EnemyLayer;


    [SerializeField] private Material DamageMat;
    [SerializeField] private Material NormalMat;
    public GameObject Projectile;

    private bool CanShoot = true;

    public float FireRate = 0.95f;
    public float RotationOffset;


    [SerializeField] private float ShotCooldownTime = 0f;


    [SerializeField] private List<GameObject> TargetList = new List<GameObject>();

    private void Start()
    {
        //MaxHealth = 40;
        TowerStartup();
        NormalMat = TowerMeshRender.materials[0];
        //TowerMaterials = TowerMeshRender.materials;

    }

    private void Update()
    {

        //TowerMeshRender.materials = TowerMaterials;
        if (TargetList.Count <= 0) { return; }
        Debug.DrawRay(FirePoint.transform.position, FirePoint.transform.forward * 3, Color.blue);
        for (int i = 0; i < TargetList.Count; i++)
        {
            //Debug.Log(TargetList[i].name);
            if (TargetList[i] == null)
            {
                TargetList.RemoveAt(i);
            }
        }
        AttackTargets();
    }

    private void AttackTargets()
    {
        if (TargetList.Count > 0)
        {
            //Vector3 RotationChange = RotateObject.transform.position-new Vector3(0,2.55f,0);
            RotateObject.transform.rotation = Quaternion.LookRotation(TargetList[0].transform.position - RotateObject.transform.position);

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

    protected override void TakeDamageEffect()
    {
        base.TakeDamageEffect();

        StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        TowerMaterials[0] = DamageMat;
        TowerMeshRender.materials = TowerMaterials;
        yield return new WaitForSeconds(0.25f);
        TowerMaterials[0] = NormalMat;
        TowerMeshRender.materials = TowerMaterials;
        //Debug.Log("her");

    }

    private void FireProjectile()
    {
        GameObject ProjectileInstance = Instantiate(Projectile, FirePoint.transform.position, Quaternion.Euler(FirePoint.transform.forward));

        Rigidbody ProjectileRB = ProjectileInstance.GetComponent<Rigidbody>();

        ProjectileRB.velocity = FirePoint.transform.forward * -150;
        //TargetList[0].GetComponent<BaseEnemy>().HandleHealth(-10);
        if (TargetList[0].GetComponent<BaseEnemy>().CurrentHealth == 0)
        {
            TargetList.RemoveAt(0);
        }
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
