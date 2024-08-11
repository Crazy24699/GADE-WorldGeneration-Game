using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInteraction : MonoBehaviour
{

    public GameObject BuildingRef;
    public GameObject HoverObject;

    public bool FollowingMouse;
    public LayerMask GroundMasks;
    public Camera ViewingCamera;

    // Start is called before the first frame update
    void Start()
    {
        ViewingCamera=Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTrackingFalse(bool Value)
    {
        //BuildPhaseScript.enabled = false;
    }

    public void SetTrackingTrue()
    {
        //BuildPhaseScript.enabled = true;
    }

    public void AssignBuilding()
    {
        Instantiate(BuildingRef,Vector3.zero,Quaternion.identity);
        return;
    }

    public void DropBuilding()
    {
        Vector3 Location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(Location.ToString());
    }


    private void MouseMovement()
    {
        Vector3 MouseScreenPosition = Input.mousePosition;
        Ray MousePositionRay = ViewingCamera.ScreenPointToRay(MouseScreenPosition);


        if (Physics.Raycast(MousePositionRay, out RaycastHit HitInfo, 150, GroundMasks))
        {
            HoverObject = HitInfo.collider.gameObject;
            MouseScreenPosition = HitInfo.point;

        }
    }
}
