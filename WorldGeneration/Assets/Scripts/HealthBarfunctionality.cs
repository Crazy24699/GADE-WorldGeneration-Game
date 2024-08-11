using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarfunctionality : MonoBehaviour
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
        transform.LookAt(MainCamera.transform.position);
    }
}
