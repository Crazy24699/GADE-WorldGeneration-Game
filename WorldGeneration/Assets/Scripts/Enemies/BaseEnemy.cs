using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public EnemySpawnerLogic ParentSpawner;
    [HideInInspector] public EnemyObject EnemyType;
    [SerializeField] protected Animator EnemyAnimator;

    protected int MaxHealth = 20;
    [SerializeField]public int CurrentHealth;

    protected float MoveSpeed;
    [SerializeField] protected ParticleSystem TakenDamageEffect;

    public int EnemyCost = 0;
    public int KillReward;
    public int Damage = 5;

    public virtual void Startup()
    {
        EnemyAnimator = GetComponent<Animator>();
        if(EnemyAnimator == null)
        {
            EnemyAnimator = gameObject.transform.root.GetComponentInChildren<Animator>();
        }

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
    }

    protected void SetAnimationBool(string AnimationName, bool AnimationState)
    {
        EnemyAnimator.SetBool(AnimationName, AnimationState);
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

}
