using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateTowers : MonoBehaviour
{

    [SerializeField] private TowerAreaCheck AreaCheckingObject;

    public GameObject CheckedAreaVisualizer;

    // Start is called before the first frame update
    void Start()
    {
        AreaCheckingObject = transform.GetComponent<TowerAreaCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }




}
