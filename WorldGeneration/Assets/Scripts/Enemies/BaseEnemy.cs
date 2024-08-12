using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{

    [SerializeField]protected int MaxHealth;
    protected int CurrentHealth;

    protected float MoveSpeed;
    [SerializeField] protected ParticleSystem TakenDamageEffect;

    public virtual void Startup()
    {
        HandleHealth(MaxHealth);

        if (MaxHealth <= 0)
        {
            Debug.LogError("Health Not Set");
        }
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
            return;
        }
        else if (CurrentHealth <= 0) 
        {
            Die();
        }
    }
    protected void Die()
    {
        ProgramManager.ProgramManagerInstance.EnemyCount.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

    protected void TakenDamage()
    {
        ParticleSystem HitEffect = Instantiate(TakenDamageEffect, transform.position, Quaternion.identity);
        HitEffect.Play();
    }

}
