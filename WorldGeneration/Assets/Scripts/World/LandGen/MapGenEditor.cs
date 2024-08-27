using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(HeightMapGenerator))]
public class MapGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HeightMapGenerator mapGen = (HeightMapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.AutoUpdate)
            {
                mapGen.GenMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenMap();
        }
    }

}
