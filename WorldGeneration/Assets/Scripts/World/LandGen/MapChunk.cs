using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapChunk : MonoBehaviour
{
    private const float ColliderGeneration = 5;

    public Vector2 ChunkCoordinate;
    private Vector2 CenterCoordinate;
    private Bounds ChunkBounds;

    private GameObject MeshGameObject;
    private MeshRenderer MeshRendererRef;
    private MeshFilter MeshFilterRef;
    private MeshCollider MeshColliderRef;

    private LODInfo[] DetailLevels;
    private 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

class MeshLOD
{
    public Mesh MeshRef;
    public bool MeshRequested;
    public bool HasMesh;

    private int LODValue;

    public UnityEvent MeshUpdateEvent;

    public MeshLOD (int LODValueRef)
    {
        LODValue = LODValueRef;
    }

    private void MeshDataUploaded(object MeshDataObject)
    {
        MeshRef = ((Mesh)MeshDataObject);
    }

}
