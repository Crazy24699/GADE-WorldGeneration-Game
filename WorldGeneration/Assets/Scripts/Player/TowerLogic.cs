using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerLogic : TowerBase
{


    public void DetermineAreaClearance(bool AreaClear)
    {
        switch (AreaClear)
        {
            case true:
                TowerStartup();
                break;
            
            case false:
                Destroy(this.gameObject);
                break;
        }
    }

    

    
}
