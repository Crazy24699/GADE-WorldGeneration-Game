using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBase : MonoBehaviour
{
    [SerializeField] private Slider HealthBar;
    [SerializeField] protected ParticleSystem TakenDamageEffect;

    [SerializeField] protected int MaxHealth;
    protected int CurrentHealth;

    // Start is called before the first frame update
    void Start()
    {
        if(ProgramManager.ProgramManagerInstance.DevMode)
        {
            TowerStartup();
            Debug.Log("Dev Mode enabled");
        }
    }

    public virtual void TowerStartup()
    {
        if (HealthBar == null)
        {
            HealthBar = transform.GetComponentInChildren<Slider>();
        }
        if (MaxHealth <= 0)
        {
            Debug.LogError("Health Not Set");
        }

        HealthBar.maxValue = MaxHealth;
        CurrentHealth = MaxHealth;
        HealthBar.value = CurrentHealth;
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
                HealthBar.value = CurrentHealth;
            }
            return;
        }
        else if (CurrentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}
