using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class Module
{
    public int ID;
    public int rotIndex;
    public string referanceMesh;
    public Dictionary<string, List<int>> validNeighbors = new Dictionary<string, List<int>>()
    {
        {"PosX", new List<int>()},
        {"NegX", new List<int>()},
        {"PosY", new List<int>()},
        {"NegY", new List<int>()},
        {"PosZ", new List<int>()},
        {"NegZ", new List<int>()},
    };



}

[System.Serializable]

public class ModuleList
{
    public List<Module> modules = new List<Module>();
}
