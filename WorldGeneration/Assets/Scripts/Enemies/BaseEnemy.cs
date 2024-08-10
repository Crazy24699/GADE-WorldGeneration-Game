using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{

    protected int MaxHealth;
    protected int CurrentHealth;

    protected float MoveSpeed;
    [SerializeField] protected ParticleSystem TakenDamageEffect;

    public virtual void Startup()
    {
        HandleHealth(MaxHealth);
    }

    public virtual void HandleHealth(int HealthChange)
    {
        if ((CurrentHealth > 0 || CurrentHealth <= MaxHealth))
        {
            CurrentHealth += HealthChange;
            if (HealthChange < CurrentHealth) 
            {
                ParticleSystem HitEffect = Instantiate(TakenDamageEffect, transform.position, Quaternion.identity);
                HitEffect.Play();
            }
            return;
        }
        else if (CurrentHealth <= 0) 
        {
            Destroy(this.gameObject);
        }
    }

    protected virtual void DealDamage()
    {

    }

}
