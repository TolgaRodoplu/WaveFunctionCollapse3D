using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Module : MonoBehaviour
{
    int ID;
    string referanceMesh;
    Dictionary<string, List<int>> validNeighbors;

    private void Start()
    {
        
    }

}
