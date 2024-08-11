using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : BaseEnemy
{

    private GameObject[] PlayerTowers;
    [SerializeField] private GameObject TowerTarget;
    [SerializeField] private NavMeshAgent AgentRef;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Startup()
    {
        base.Startup();
        Debug.Log("startup");
        AgentRef = GetComponent<NavMeshAgent>();
        FindNearestTower();
        AgentRef.SetDestination(TowerTarget.transform.position);
    }

    private void FindNearestTower()
    {
        PlayerTowers = GameObject.FindGameObjectsWithTag("PlayerTower");
        float MinDistance = 0;

        foreach (var Tower in PlayerTowers)
        {
            float TowerDistance = Vector3.Distance(transform.position, Tower.transform.position);
            TowerDistance = Mathf.Abs(TowerDistance);
            if (MinDistance == 0)
            {
                MinDistance = TowerDistance;
                TowerTarget = Tower;
            }
            if (TowerDistance < MinDistance)
            {
                TowerTarget = Tower;
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            Startup();
        }
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("PlayerTower"))
        {

            Collision.GetComponent<TowerLogic>().HandleHealth(-CurrentHealth);
            TakenDamage();
            Destroy(gameObject);
        }
    }

}
