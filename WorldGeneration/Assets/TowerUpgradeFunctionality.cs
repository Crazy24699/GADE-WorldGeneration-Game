using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerUpgradeFunctionality : MonoBehaviour
{

    public TextMeshProUGUI HealthCost;
    public TextMeshProUGUI DamageCost;

    public GameObject HealthButton;
    public GameObject DamageButton;
    public GameObject UpgradePanel;

    public TowerBase TowerBaseScriptLink;
    public TowerBase.UpgradeOptions ChosenOption;

    private void Start()
    {
        TowerBaseScriptLink=this.GetComponent<TowerBase>();

        HealthButton.SetActive(true);
        DamageButton.SetActive(true);
        UpgradePanel.SetActive(false);



    }


    public void SelectTower()
    {
        UpgradePanel.SetActive(true);

    }

    public void UpgradeDamage()
    {
        if (ChosenOption == TowerBase.UpgradeOptions.None)
        {
            ChosenOption = TowerBase.UpgradeOptions.Damage;
            TowerBaseScriptLink.ChosenUpgradePath = TowerBase.UpgradeOptions.Damage;
            HealthButton.SetActive(false);
            TowerBaseScriptLink.SetShader(TowerBase.UpgradeOptions.Damage);
        }

        TowerBaseScriptLink.UpgradeTowerDamage();
        DeselectTower();
    }

    public void UpgradeHealth()
    {
        if (ChosenOption == TowerBase.UpgradeOptions.None)
        {
            Debug.Log("disconnect");
            ChosenOption = TowerBase.UpgradeOptions.Health;
            TowerBaseScriptLink.ChosenUpgradePath = TowerBase.UpgradeOptions.Health;
            DamageButton.SetActive(false);
            TowerBaseScriptLink.SetShader(TowerBase.UpgradeOptions.Health);
        }

        TowerBaseScriptLink.UpgradeTowerHealth();
        DeselectTower();
    }

    public void DeselectTower()
    {
        UpgradePanel.SetActive(false);
    }

}
