using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Cell 
{
    public Vector3Int index;
    public int ID = -10;
    public List<int> possibleModules;
        
    public Cell(Vector3Int v)
    {
        index = v;

        possibleModules = new List<int>();
        
    }

    //bool ModifyPossibilities(List<int> possibilities)
    //{
    //    List<int> result = new List<int>();


    //} 

}

