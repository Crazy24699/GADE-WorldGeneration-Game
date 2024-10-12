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
    protected override void AlotMoney()
    {
        Debug.Log("aaaaaaaaaaaaa");
        FindObjectOfType<PlayerHandler>().HandleMoney(KillReward);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
