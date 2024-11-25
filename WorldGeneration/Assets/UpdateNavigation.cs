using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class UpdateNavigation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            NavMeshSurface[] Surfaces = FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);
            foreach (NavMeshSurface Surface in Surfaces)
            {
                //Surface.RemoveData();
                //Surface.AddData();
                Surface.BuildNavMesh();
                Debug.Log("alive");
            }
        }
    }
}
