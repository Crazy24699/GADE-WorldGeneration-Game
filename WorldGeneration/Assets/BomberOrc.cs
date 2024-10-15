using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BomberOrc : BaseEnemy
{


    [SerializeField] public GameObject FinalTarget;
    [SerializeField] public GameObject CurrentTarget;
    [SerializeField] private GameObject ThrowObject;

    [SerializeField] public Transform ThrowPoint;
    [SerializeField] private NavMeshAgent AgentRef;



    private float ShotCooldownTime = 0f;
    public float FireRate = 0.95f;
    [SerializeField] private float MaxAttackDistance;
    public float LookSpeed;

    private DefenderTower AttackTarget;

    private bool StartupRan = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Startup()
    {
        AgentRef = GetComponent<NavMeshAgent>();
        AgentRef.enabled = true;
        KillReward = 5;
        base.Startup();
        //Debug.Log("startup");
        if (MaxAttackDistance <= 0) { MaxAttackDistance = 19.75f; }

        StartCoroutine(AdditionalStartup());
    }
    public IEnumerator AdditionalStartup()
    {
        yield return new WaitForSeconds(0.25f);
        AgentRef = GetComponent<NavMeshAgent>();
        AgentRef.enabled = true;

        yield return new WaitForSeconds(0.55f);
        AgentRef.SetDestination(FinalTarget.transform.position);
        CurrentTarget = FinalTarget;

        StartupRan = true;
        if (EnemyCost == 0)
        {
            Debug.LogError("Cost not set        " + this.gameObject.name);
        }
    }

    private void AttackTower()
    {
        if (AttackTarget == null)
        {
            return;
        }

        SetAnimationBool("Attack", true);
        SetAnimationBool("Walk", false);

        ShotCooldownTime += Time.deltaTime;
        if (ShotCooldownTime >= FireRate)
        {
            Throw();
            Debug.Log("aaaaa");
            ShotCooldownTime = 0f;
        }
    }

    private void TrackTarget()
    {
        if (CurrentTarget == null) { return; }

        Vector3 TargetDirection = CurrentTarget.transform.position - this.transform.position;

        //Sets the rotation of the 
        Quaternion ViewingRotation = Quaternion.LookRotation(TargetDirection);
        transform.rotation = ViewingRotation;
    }

    private void Throw()
    {
        GameObject ProjectileInstance = Instantiate(ThrowObject, ThrowPoint.transform.position, ThrowPoint.transform.rotation);

        Rigidbody ProjectileRB = ProjectileInstance.GetComponent<Rigidbody>();

        ProjectileRB.velocity = ThrowPoint.transform.forward * 50;
        //TargetList[0].GetComponent<BaseEnemy>().HandleHealth(-10);


    }

    protected override void AlotMoney()
    {
        Debug.Log("aaaaaaaaaaaaa");
        FindObjectOfType<PlayerHandler>().HandleMoney(KillReward);
    }

    private void Update()
    {
        if (!StartupRan)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AgentRef.SetDestination(FinalTarget.transform.position);
            Debug.Log("Nice try but two can play this game");
        }
        TrackTarget();
        AttackTower();

        if (CurrentTarget == null) { CurrentTarget = FinalTarget; AgentRef.SetDestination(FinalTarget.transform.position); }

        //AgentRef.SetDestination(TowerTarget.transform.position);
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("PlayerTower"))
        {
            Collision.GetComponent<TowerLogic>().HandleHealth(-CurrentHealth);
            TakenDamage();
            Destroy(this.gameObject);
        }
        if (Collision.CompareTag("DefenderTower"))
        {
            AttackTarget = Collision.GetComponent<DefenderTower>();
            CurrentTarget = Collision.gameObject;
            NavMeshHit NavMeshHitInfo;
            if (NavMesh.SamplePosition(CurrentTarget.transform.position, out NavMeshHitInfo, 50.0f, NavMesh.AllAreas))
            {
                //AgentRef.SetDestination(NavMeshHitInfo.position);
            }
        }
    }

}
