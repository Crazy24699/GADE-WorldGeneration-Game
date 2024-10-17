using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : BaseEnemy
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Startup()
    {
        AgentRef = GetComponent<NavMeshAgent>();


        AgentRef.enabled = true;
        KillReward = 5;
        MaxHealth = 25;
        MoveSpeed = 5f;

        base.Startup();
        //Debug.Log("startup");
        if (MaxAttackDistance <= 0) { MaxAttackDistance = 19.75f; }

        StartCoroutine(AdditionalStartup());
    }

    protected override void AlotMoney()
    {
        Debug.Log("aaaaaaaaaaaaa");
        FindObjectOfType<PlayerHandler>().HandleMoney(KillReward);
    }

    public IEnumerator AdditionalStartup()
    {
        yield return new WaitForSeconds(0.25f);
        AgentRef = GetComponent<NavMeshAgent>();
        AgentRef.enabled = true;
        AgentRef.SetDestination(FinalTarget.transform.position);
        AgentRef.speed = MoveSpeed;

        CurrentTarget = FinalTarget;
        StartupRan = true;
        if (EnemyCost == 0)
        {
            //Debug.LogError("Cost not set        " + this.gameObject.name);
        }
    }



    private void Update()
    {
        if (!StartupRan)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            AgentRef.SetDestination(FinalTarget.transform.position);
            Debug.Log("Nice try but two can play this game");
        }

        if (!Attacking)
        {
            SetAnimationBool("Walking", true);
            SetAnimationBool("Attacking", false);
        }

        TrackTarget();
        AttackTower();

        if (CurrentTarget == null) {CurrentTarget=FinalTarget; AgentRef.SetDestination(FinalTarget.transform.position); }

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
                AgentRef.SetDestination(NavMeshHitInfo.position);
            }
        }
    }

    private void OnTriggerExit(Collider Collision)
    {
        if (Collision.CompareTag("DefenderTower"))
        {
            CurrentTarget = FinalTarget;
        }
    }

}
