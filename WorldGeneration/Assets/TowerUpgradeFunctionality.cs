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
            HealthButton.SetActive(false);
        }

        TowerBaseScriptLink.UpgradeTowerDamage();

    }

    public void UpgradeHealth()
    {
        if (ChosenOption == TowerBase.UpgradeOptions.None)
        {
            ChosenOption = TowerBase.UpgradeOptions.Health;
            DamageButton.SetActive(false);
        }

        TowerBaseScriptLink.UpgradeTowerHealth();

    }

    public void DeselectTower()
    {
        UpgradePanel.SetActive(false);
    }

}
