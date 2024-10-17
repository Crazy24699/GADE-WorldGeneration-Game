using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    public EnemySpawnerLogic ParentSpawner;
    [HideInInspector] public EnemyObject EnemyType;
    [SerializeField] protected Animator EnemyAnimator;
    [SerializeField] public GameObject FinalTarget;
    [SerializeField] public GameObject CurrentTarget;


    [SerializeField] protected GameObject ThrowObject;

    [SerializeField] public Transform ThrowPoint;
    [SerializeField] protected NavMeshAgent AgentRef;

    [SerializeField] protected float Distance;


    protected float ShotCooldownTime = 0f;
    public float FireRate = 0.95f;
    [SerializeField] protected float MaxAttackDistance;
    public float LookSpeed;

    [SerializeField] protected DefenderTower AttackTarget;

    protected bool StartupRan = false;
    [SerializeField]protected bool Attacking = false;

    protected int MaxHealth = 0;
    [SerializeField]public int CurrentHealth;
    protected float MoveSpeed = 0;
    [SerializeField]protected float AttackRange = 0;

    [SerializeField] protected ParticleSystem TakenDamageEffect;

    public int EnemyCost = 0;
    public int KillReward;
    public int Damage = 5;

    public virtual void Startup()
    {
        if (KillReward <= 0)
        {
            Debug.LogError("Killreward not set");
            KillReward = 1;
        }
        HandleHealth(MaxHealth);

        if (MaxHealth <= 0)
        {
            Debug.LogError("Health Not Set");
        }
        SetAgentValues();
        EnemyAnimator = GetComponent<Animator>();
        if(EnemyAnimator == null)
        {
            EnemyAnimator = transform.root.GetComponentInChildren<Animator>();
        }
        SetAnimationBool("Walking", true);
    }

    public virtual void HandleHealth(int HealthChange)
    {
        if ((CurrentHealth > 0 || CurrentHealth <= MaxHealth))
        {
            CurrentHealth += HealthChange;
            if (HealthChange < CurrentHealth)
            {
                TakenDamage();
            }

        }
        if (CurrentHealth <= 0)
        {

            Die();
        }
    }

    protected void SetAgentValues()
    {
        AgentRef.speed = MoveSpeed;
        AttackRange = AgentRef.stoppingDistance;
    }

    protected void HandleStoppingDistance()
    {
        if (!StartupRan) { return; }
        if(CurrentTarget==FinalTarget)
        {
            AgentRef.stoppingDistance = 0;
        }
        else if (CurrentTarget!=FinalTarget)
        {
            AgentRef.stoppingDistance = AttackRange;
        }
    }

    private void LateUpdate()
    {
        HandleStoppingDistance();
    }

    protected void AttackTower()
    {
        if (AttackTarget == null)
        {
            Attacking = false;
            return;
        }

        Distance = Vector3.Distance(transform.position, AttackTarget.transform.position);
        if(Distance > AttackRange+10)
        {
            return;
        }

        SetAnimationBool("Walking", false);


        Attacking = true;
        ShotCooldownTime += Time.deltaTime;
        if (ShotCooldownTime >= FireRate)
        {
            SetAnimationBool("Attacking", true);
            Throw();
            Debug.Log("Attacking"+this.gameObject.name);
            ShotCooldownTime = 0f;
        }
    }

    protected void TrackTarget()
    {
        if (CurrentTarget == null) { return; }

        Vector3 TargetDirection = CurrentTarget.transform.position - this.transform.position;
        TargetDirection.y = 0;
        //Sets the rotation of the 
        Quaternion ViewingRotation = Quaternion.LookRotation(TargetDirection);
        transform.rotation = ViewingRotation;
    }

    protected void Throw()
    {
        GameObject ProjectileInstance = Instantiate(ThrowObject, ThrowPoint.transform.position, ThrowPoint.transform.rotation);

        Rigidbody ProjectileRB = ProjectileInstance.GetComponent<Rigidbody>();

        ProjectileRB.velocity = ThrowPoint.transform.forward * 50;

        //TargetList[0].GetComponent<BaseEnemy>().HandleHealth(-10);


    }

    public void PopulateValues(EnemyObject SpawnedType)
    {
        EnemyType = SpawnedType.SpawnedEnemyObject();
    }


    protected void Die()
    {
        ProgramManager.ProgramManagerInstance.EnemyCount.Remove(this.gameObject);
        ParentSpawner.UpdateEnemies(EnemyType);
        AlotMoney();
        Destroy(this.gameObject);
    }

    protected virtual void AlotMoney()
    {

    }

    protected void TakenDamage()
    {
        ParticleSystem HitEffect = Instantiate(TakenDamageEffect, transform.position, Quaternion.identity);
        HitEffect.Play();
    }



    protected void SetAnimationBool(string AnimationName, bool AnimationState)
    {
        EnemyAnimator.SetBool(AnimationName, AnimationState);
    }

}
