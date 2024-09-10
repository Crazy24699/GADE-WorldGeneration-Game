using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TowerLogic : TowerBase
{
    public PlayerUI PlayerUIScript;

    protected override void Die()
    {
        CurrentHealth = 0;
        PlayerUIScript.Defeat();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            HandleHealth(-CurrentHealth);
        }
    }



}
