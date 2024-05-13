using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class Generator : MonoBehaviour
{
    ModuleList moduleList;
    public int dimX = 100;
    public int dimY = 10;
    public int dimZ = 100;
    Cell[,,] map;
    Dictionary<string, Mesh[]> meshes; 
    Queue<Cell> cellsOrder = new Queue<Cell>();
    public float renderInterval = 0.1f;
    public string emptyMesh = null;
    public string undergroundMesh = null;
    public int initilizeChoice = 51;
    string meshPathName = "Objects/";

    private void Start()
    {
        moduleList = SaveData.LoadFromJson();
        meshes = new Dictionary<string, Mesh[]>();  

        InitilizeMap();
        InitilizeEdges();
        WaveFunction();
        StartCoroutine(RenderInTime());
    }

   

    //Initilize maps edges to specified tile.
    void InitilizeEdges()
    {
        foreach (Cell cell in map)
        {
            

            if (cell.index.x == 0 || cell.index.x == dimX - 1 || cell.index.z == 0 || cell.index.z == dimZ - 1)
            {
                if ((cell.index.y == 0))
                {
                    Collapse(cell.index, initilizeChoice);
                }
            }

            ModifyNeighbors(cell.index);
        }
    }

    //Selects a id from a given list based on their weights

    int SelectRandom(List<int> possibleModules)
    {

        List<int> selectionList = new List<int>();

        foreach (int i in possibleModules)
        {
            int weight = moduleList.modules[i].weight;

            for(int j = 0; j < weight; j++)
                selectionList.Add(i);
        }

        int rand = Random.Range(0, selectionList.Count);

        int x = selectionList[rand];

        return x;

    }

    //Main Algorithm Loop

    void WaveFunction()
    {
        while (!isCollapsedComplate())
            Iterate();

        Debug.Log("AllCellsCollapsed!!!!!!");
    }

    //Algorithm iteration

    void Iterate()
    {
        Vector3Int lowestEnt = FindLowestEntropyIndex();
        Collapse(lowestEnt);
        ModifyNeighbors(lowestEnt);
    }

    //Collapses the given cell randomly
    void Collapse(Vector3Int collapsedCell)
    {
        Cell c = GetCell(collapsedCell);

        int x = SelectRandom(c.possibleModules);



        c.ID = x;

        c.possibleModules.Clear();
        c.possibleModules.Add(x);

        AddToRenderQueue(c);

        //RenderCell(c);

    }

    //Adds cells to a queue to be rendered for demonstration
    void AddToRenderQueue(Cell c)
    {
        if (!moduleList.modules[c.ID].referanceMesh.Equals(emptyMesh) && !moduleList.modules[c.ID].referanceMesh.Equals(undergroundMesh))
            cellsOrder.Enqueue(c);
    }

    //Collapses cell to a determined state
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

            AddToRenderQueue(c);

            //RenderCell(c);
        }
    }


    //Take Collapsed cell and iterate over its neighbors and their neighbors and so on, to distribute the neccecary changes on their possibility lists
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

    //Returns a copy of a cells possibility list
    List<int> GetPossibilitiesCopy(Vector3Int cellCoord)
    {
        List<int> result = new List<int>(GetCell(cellCoord).possibleModules);
        return result;
    }


    //Make changes on the cell to remove unwanted ids
    void Constrain(Vector3Int cellCoords, int p)
    {
        Cell cell = GetCell(cellCoords);

        cell.possibleModules.RemoveAll(r => r == p);
    }
    
    
    //Returns a string based on a given vector
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


    //Returns a new list consisting of intersection of the possibilities of all the possible mopdules neighbors in specified direction
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

    //Returns a list of valid directions
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

    //Returns the cell in a given coordinate
    Cell GetCell(Vector3Int index)
    {
        return map[index.x, index.y, index.z];
    }


    //Finds the cell with the lowest number of possible modules and returns its coordinates
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

    //Checks if all the cells are collapsed 
    bool isCollapsedComplate()
    {
        foreach (Cell c in map) 
        {

            if (c.ID == -10)
                return false;

        }
        return true;
    }

    //Initilizes the map and puts a cell object and initilizes all their possibilities to all the possible modules 
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

                    foreach (Module l in moduleList.modules)
                    {
                        map[i, j, k].possibleModules.Add(l.ID);
                    }
                }
            }
        }

        
    }

    //Render the queued objects in a interval
    IEnumerator RenderInTime()
    {
        while (cellsOrder.Count > 0) 
        {
            RenderCell(cellsOrder.Dequeue());

            yield return new WaitForSeconds(renderInterval);
        }
    }

    //Render the given cell
    private void RenderCell(Cell c)
    {
        if (!meshes.ContainsKey(moduleList.modules[c.ID].referanceMesh))
        {
            Mesh[] meshFolderContent = Resources.LoadAll<Mesh>(meshPathName + moduleList.modules[c.ID].referanceMesh);
            meshes.Add(moduleList.modules[c.ID].referanceMesh, meshFolderContent);
        }

        int rand = Random.Range(0, meshes[moduleList.modules[c.ID].referanceMesh].Length);

        Mesh m = meshes[moduleList.modules[c.ID].referanceMesh][rand];

        GameObject obj = Instantiate((GameObject)Resources.Load("Cell", typeof(GameObject)));

        obj.GetComponent<MeshFilter>().mesh = m;

        Vector3 rot = new Vector3(0, 90 * moduleList.modules[c.ID].rotIndex, 0);

        Vector3 pos = c.index * 2;

        obj.transform.position = pos;

        obj.transform.Rotate(rot);
        
       
    }
}
