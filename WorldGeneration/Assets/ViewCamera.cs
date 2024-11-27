using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCamera : MonoBehaviour
{
    public Camera MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(MainCamera.transform.position);
    }
}
