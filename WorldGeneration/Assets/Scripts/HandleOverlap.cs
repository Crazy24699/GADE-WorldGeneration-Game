using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleOverlap : MonoBehaviour
{
    public bool AreaClear = true;

    public Material OpenOutline;
    public Material BlockedOutline;

    public Renderer OutlinerRenderer;

    private void Start()
    {
        OutlinerRenderer=GetComponent<Renderer>();
    }

    private void OnTriggerStay(Collider Collision)
    {
        if (Collision.CompareTag("Pathway"))
        {
            AreaClear = false;
            OutlinerRenderer.material = BlockedOutline;
            return;
        }
        
    }
    private void OnTriggerExit(Collider Collision)
    {
        if (Collision.CompareTag("Pathway"))
        {
            AreaClear = true;
            OutlinerRenderer.material = OpenOutline;
            return;
        }
    }
}
