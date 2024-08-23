using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3Int Coordinate;

    public Mesh ChunkMesh;

    private MeshFilter ChunkMeshFilter;
    private MeshRenderer ChunkRenderer;
    private MeshCollider ChunkCollider;

    public void CreateChunk(Material ChunkMaterial)
    {
        ChunkMeshFilter = ChunkMeshFilter == null ? gameObject.AddComponent<MeshFilter>() : GetComponent<MeshFilter>();
        ChunkRenderer = ChunkRenderer == null ? gameObject.AddComponent<MeshRenderer>() : GetComponent<MeshRenderer>();
        ChunkCollider = ChunkCollider ? gameObject.AddComponent<MeshCollider>() : GetComponent<MeshCollider>();

        ChunkMesh = ChunkMeshFilter.sharedMesh;
        if(ChunkMesh == null)
        {
            ChunkMesh = new Mesh();
            ChunkMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            ChunkMeshFilter.sharedMesh = ChunkMesh;
        }

        ChunkCollider.enabled = false;
        ChunkCollider.enabled = true;

        ChunkRenderer.material = ChunkMaterial;
    }

    public void DestoryChunk()
    {
        if (Application.isPlaying)
        {
            ChunkMesh.Clear();
            this.gameObject.SetActive(false);
        }
    }

}
