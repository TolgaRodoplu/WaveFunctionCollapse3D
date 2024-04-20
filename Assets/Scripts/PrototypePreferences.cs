using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class PrototypePreferences : MonoBehaviour
{
    [HideInInspector]
    public string meshName = null;
    public int weight = 1;

    private void Start()
    {
        meshName = GetComponent<MeshFilter>().mesh.name;

        if (meshName.EndsWith(" Instance"))
        {
            meshName = meshName.Substring(0, meshName.LastIndexOf(" Instance"));
        }
    }
}
