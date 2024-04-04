using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Generator : MonoBehaviour
{
    ModuleList moduleList;
    public int dimX = 20;
    public int dimY = 5;
    public int dimZ = 20;
    int edgeModuleChoice = 28;
    Cell[,,] map;
    Mesh[] meshes;

    private void Start()
    {
        moduleList = SaveData.LoadFromJson();
        meshes = Resources.LoadAll<Mesh>(moduleList.fbxName);
        
        InitilizeMap();
        InitilizeEdges();
        WaveFunction();

    }

    void InitilizeEdges()
    {
        foreach (Cell cell in map)
        {

            if (cell.index.x == 0 || cell.index.x == dimX - 1 || cell.index.z == 0 || cell.index.z == dimZ - 1)
            {
                if ((cell.index.y == 2))
                {
                    Collapse(cell.index, edgeModuleChoice);
                }
            }

            ModifyNeighbors(cell.index);
        }
    }

    void WaveFunction()
    {
        while (!isCollapsedComplate())
            Iterate();

        Debug.Log("AllCellsCollapsed!!!!!!");
    }

    void Iterate()
    {
        Vector3Int lowestEnt = FindLowestEntropyIndex();
        Collapse(lowestEnt);
        ModifyNeighbors(lowestEnt);
    }

    void Collapse(Vector3Int collapsedCell)
    {
        Cell c = GetCell(collapsedCell);

        int rand = Random.Range(0, c.possibleModules.Count);

        int x = c.possibleModules[rand];

        c.ID = x;

        c.possibleModules.Clear();
        c.possibleModules.Add(x);

        RenderCell(c);

    }

    void Collapse(Vector3Int collapsedCell, int choice)
    {
        Cell c = GetCell(collapsedCell);

        if(c.possibleModules.Contains(choice))
        {
            c.ID = choice;

            c.possibleModules.Clear();
            c.possibleModules = new List<int>();
            c.possibleModules.Add(choice);
            ModifyNeighbors(collapsedCell);
            RenderCell(c);
        }
    }

    void ModifyNeighbors(Vector3Int collapsedCell)
    {

        Stack<Vector3Int> toBeModified = new Stack<Vector3Int>();
        toBeModified.Push(collapsedCell);

        while (toBeModified.Count > 0) 
        {
            Vector3Int currentCellCoord = toBeModified.Pop();

            foreach (Vector3Int d in GetValidDirections(currentCellCoord))
            {
                Vector3Int otherCellCoord = currentCellCoord + d;

                if (GetCell(otherCellCoord).possibleModules.Count == 1)
                    continue;

                var otherCellPossibilities = GetPossibilitiesCopy(otherCellCoord);

                var possibleNeighborsOfCurrentInDir = GetPossibleNeighborsInDir(GetCell(currentCellCoord).possibleModules, d);

                foreach(int p in otherCellPossibilities)
                {
                    if(!possibleNeighborsOfCurrentInDir.Contains(p))
                    {
                        Constrain(otherCellCoord, p);

                        if(!toBeModified.Contains(otherCellCoord))
                        {
                            toBeModified.Push(otherCellCoord);
                        }
                    }
                }

                
            }



        }

    }

    List<int> GetPossibilitiesCopy(Vector3Int cellCoord)
    {
        List<int> result = new List<int>(GetCell(cellCoord).possibleModules);
        return result;
    }

    void Constrain(Vector3Int cellCoords, int p)
    {
        Cell cell = GetCell(cellCoords);
        

        cell.possibleModules.Remove(p);
    }
    
    string GetDirString(Vector3Int dir)
    {

        if (dir == Vector3Int.right)
            return "PosX";
        if (dir == Vector3Int.left)
            return "NegX";
        if (dir == Vector3Int.up)
            return "PosY";
        if (dir == Vector3Int.down)
            return "NegY";
        if (dir == Vector3Int.forward)
            return "PosZ";
        if (dir == Vector3Int.back)
            return "NegZ";

        return null;
    }

    List<int> GetPossibleNeighborsInDir(List<int> possibleModules, Vector3Int dir) 
    {
        List<int> result = new List<int>();

        string dirString = GetDirString(dir);

        foreach (int i in possibleModules)
        {
            List<int> module = moduleList.modules[i].validNeighbors[dirString];

            foreach (int j in module)
            {
                if(!result.Contains(j))
                    result.Add(j);
            }
        }

        return result;  
    }

    List<Vector3Int> GetValidDirections(Vector3Int currentCell)
    {
        
        List<Vector3Int> result = new List<Vector3Int>();

        

        if (currentCell.x + 1 < dimX) 
        {
            result.Add(Vector3Int.right);
        }
        if (currentCell.x - 1 >= 0)
        {
            result.Add(Vector3Int.left);
        }
        if (currentCell.y + 1 < dimY)
        {
            result.Add(Vector3Int.up);
        }
        if (currentCell.y - 1 >= 0)
        {
            result.Add(Vector3Int.down);
        }
        if (currentCell.z + 1 < dimZ)
        {
            result.Add(Vector3Int.forward);
        }
        if (currentCell.z - 1 >= 0)
        {
            result.Add(Vector3Int.back);
        }

        return result;
    }

    Cell GetCell(Vector3Int index)
    {
        return map[index.x, index.y, index.z];
    }

    Vector3Int FindLowestEntropyIndex()
    {
        int lowest = int.MaxValue;

        List<Vector3Int> list = new List<Vector3Int>();

        foreach (Cell cell in map) 
        {
            if (cell.ID != -10)
                continue;

            if(cell.possibleModules.Count == lowest) 
                list.Add(cell.index);

            if(cell.possibleModules.Count < lowest )
            {
                list.Clear();
                list.Add(cell.index);
                lowest = cell.possibleModules.Count;
            }
        }

       

        return list[Random.Range(0, list.Count)];
    }

    bool isCollapsedComplate()
    {
        foreach (Cell c in map) 
        {

            if (c.ID == -10)
                return false;

        }
        return true;
    }

    void InitilizeMap()
    {
        moduleList = SaveData.LoadFromJson();

        map = new Cell[dimX, dimY, dimZ];

        for (int i = 0; i < dimX; i++)
        {
            for (int j = 0; j < dimY; j++)
            {
                for (int k = 0; k < dimZ; k++)
                {
                    Vector3Int v = new Vector3Int(i, j, k);
                    map[i, j, k] = new Cell(v);

                    for (int l = 0; l < moduleList.modules.Count; l++)
                    {
                        map[i, j, k].possibleModules.Add(l);
                    }
                }
            }
        }

        
    }

    private void RenderCell(Cell c)
    {
        GameObject obj = Instantiate((GameObject)Resources.Load("Cell", typeof(GameObject)));

        Mesh m = null;

        foreach(Mesh mesh in meshes)
        {
            if(mesh.name == moduleList.modules[c.ID].referanceMesh) 
            {
                m = mesh;
            }
        }

        obj.GetComponent<MeshFilter>().mesh = m;

        Vector3 rot = new Vector3(0, 90 * moduleList.modules[c.ID].rotIndex, 0);

        Vector3 pos = c.index * 2;

        obj.transform.position = pos;

        obj.transform.Rotate(rot);
        
       
    }
}
