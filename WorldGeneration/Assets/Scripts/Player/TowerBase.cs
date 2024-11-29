using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBase : MonoBehaviour
{
    [SerializeField] private Slider HealthBar;
    [SerializeField] protected ParticleSystem TakenDamageEffect;

    [SerializeField] protected int MaxHealth = 20;
    [SerializeField]protected int CurrentHealth;
    [SerializeField] protected int Damage;
     protected bool  StartupRan=false;
    public TowerUpgradeFunctionality TowerUpgradeScript;

    public Material DamageUpShader;
    public Material HealthUpShader;
    public Material[] TowerMaterials;
    public List<Material> DumbMaterials;
    [SerializeField] public MeshRenderer TowerMeshRender;
    public int BasePrice;
    public int CurrentUpgradeCost;

    public enum UpgradeOptions
    {
        None,
        Health,
        Damage
    }

    public UpgradeOptions ChosenUpgradePath;

    //[SerializeField] protected GameObject EnemiesInRange;

    // Start is called before the first frame update
    void Start()
    {

        CurrentHealth = MaxHealth;
        //TowerMaterials = new Material[5];
        //TowerMaterials = TowerMeshRender.materials;
        if (ProgramManager.ProgramManagerInstance.DevMode)
        {
            TowerStartup();
            Debug.Log("Dev Mode enabled");
        }

    }

    public virtual void TowerStartup()
    {
        Material[] materials = TowerMeshRender.materials;
        Debug.Log(materials.Length);
        DumbMaterials = new List<Material>(materials);


        TowerUpgradeScript = this.gameObject.GetComponent<TowerUpgradeFunctionality>();
        StartupRan = false;
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
        StartupRan = true;
    }

    public void SetShader(UpgradeOptions Upgrade)
    {
        switch (Upgrade)
        {
            case UpgradeOptions.Health:
                DumbMaterials[2] = HealthUpShader;
                Debug.Log("unspoken");
                break;

            case UpgradeOptions.Damage:
                DumbMaterials[2] = DamageUpShader;
                Debug.Log("deserve the silence of a disconnected life");
                break;

        }

        TowerMeshRender.materials = DumbMaterials.ToArray();
    }

    public virtual void HandleHealth(int HealthChange)
    {
        if ((CurrentHealth > 0 || CurrentHealth <= MaxHealth))
        {
            CurrentHealth += HealthChange;
            if (HealthChange < CurrentHealth)
            {
                TakeDamageEffect();
                HealthBar.value = CurrentHealth;
                
            }
        }
        if (CurrentHealth <= 0)
        {
            Die();
            
        }
    }

    public void UpgradeTowerDamage()
    {
        CurrentUpgradeCost += BasePrice;
        Damage += 10;
        //TowerMaterials[1] = DamageUpShader;
    }

    public void UpgradeTowerHealth()
    {
        CurrentUpgradeCost += BasePrice;

        MaxHealth += 15;
        CurrentHealth = MaxHealth;
        HealthBar.value = CurrentHealth;
        //TowerMaterials[2] = HealthUpShader;
    }



    protected virtual void TakeDamageEffect()
    {
        ParticleSystem HitEffect = Instantiate(TakenDamageEffect, transform.position, Quaternion.identity);
        HitEffect.Play();
    }

    protected void DetectEnemyUnits()
    {

    }

    protected virtual void Attack()
    {

    }

    protected virtual void Die()
    {
        Destroy(this.transform.parent.gameObject);
    }

}
