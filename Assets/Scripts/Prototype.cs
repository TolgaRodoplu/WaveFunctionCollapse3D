using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype
{
    public int ID;
    public string meshName;
    public int rotIndex;
    public Dictionary<string, string> sockets = new Dictionary<string, string>()
    {
        {"PosX", null},
        {"NegX", null},
        {"PosY", null},
        {"NegY", null},
        {"PosZ", null},
        {"NegZ", null},
    };
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
