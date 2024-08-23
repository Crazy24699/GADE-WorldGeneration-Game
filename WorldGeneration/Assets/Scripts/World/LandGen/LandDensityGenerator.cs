using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandDensityGenerator : MonoBehaviour
{
    private const int ThreadSize = 4;

    //A compute shader exists to apply colour and or texture to a mesh and its parts
    //I did not come up with this code but i think i understand it. 
    [SerializeField] private ComputeShader NoiseComputeRef;

    //Compute buffers are used to write data into the GPU shaders and converts compute shaders into normal shaders
    protected List<ComputeBuffer> DensityBufferList;

    //used to update the mesh within the editor itself
    private void OnValidate()
    {
        if(FindObjectOfType<MeshGenerator>())
        {
            FindObjectOfType<MeshGenerator>().UpdateMesh();
        }
    }

    public virtual ComputeBuffer GenerateMesh(ComputeBuffer BufferPoints, int PointsPerAxis, float BoundarySize, Vector3 WorldBoundary,Vector3 MeshCenter, Vector3 MeshOffset, float Spacing)
    {
        

        //Need to get the smallest int equal or greater to the sum result
        int ThreadPerAxisNum=Mathf.CeilToInt(ThreadSize);

        return BufferPoints;
    }

}
