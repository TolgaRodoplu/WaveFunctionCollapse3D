using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class FindSockets : MonoBehaviour
{
    int verticalSocketNumberCount = 0;
    int socketNumberCount = 0;
    float socketDistance = 0.99f;
    public string fbxName = null;
    //public Transform meshesParent;
    Dictionary<string, List<Vector2>> socketList;
    Dictionary<string, List<Vector2>> socketListVertical;
    int prototypeCount = 0;
    List<Prototype> prototypes = new List<Prototype>();


    private void Start()
    {
        ModuleList moduleList = new ModuleList();

        Mesh[] meshes = Resources.LoadAll<Mesh>(fbxName); 

        socketList = new Dictionary<string, List<Vector2>>();
        socketListVertical = new Dictionary<string, List<Vector2>>();

        foreach (Mesh m in meshes) 
        {

            var sockets = LabelSocket(m.vertices);

            CreatePrototypes(sockets, m.name);

        }

        //for (int i = 0; i < meshesParent.childCount; i++)
        //{

        //    var child = meshesParent.GetChild(i);

        //    var sockets = LabelSocket(child.GetComponent<MeshFilter>().mesh.vertices);

        //    CreatePrototypes(sockets, child.name);

        //}

        

       


        //Fill valid neighbors
        foreach (Prototype p in prototypes)
        {
            var m = new Module();
            m.ID = p.ID;
            m.rotIndex = p.rotIndex;
            m.referanceMesh = p.meshName;
            foreach (Prototype p1 in prototypes)
            {
                if (isValidNeighbor(p.sockets["PosX"], p1.sockets["NegX"]))
                    m.validNeighbors["PosX"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["NegX"], p1.sockets["PosX"]))
                    m.validNeighbors["NegX"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["PosY"], p1.sockets["NegY"]))
                    m.validNeighbors["PosY"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["NegY"], p1.sockets["PosY"]))
                    m.validNeighbors["NegY"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["PosZ"], p1.sockets["NegZ"]))
                    m.validNeighbors["PosZ"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["NegZ"], p1.sockets["PosZ"]))
                    m.validNeighbors["NegZ"].Add(p1.ID);
            }

            moduleList.modules.Add(m);
        }

        //int cnt = 0;

        //for (int i = 0; i < prototypeCount / 4; i++)
        //{
        //    for (int j = 0; j < 4; j++)
        //    {
        //        RenderPrototypes(prototypes[cnt], j, i);
        //        cnt++;
        //    }
        //}

        //savePrototypesToJson

        moduleList.fbxName = fbxName;

        SaveData.SaveToJson(moduleList);

    }

    //void RenderPrototypes(Prototype p, int x, int z)
    //{
    //    GameObject obj = Instantiate((GameObject)Resources.Load("Prototype", typeof(GameObject)));
    //    GameObject obj2 = Instantiate((GameObject)Resources.Load("Objects/" + p.meshName, typeof(GameObject)), obj.transform);

    //    foreach (KeyValuePair<string, string> pair in p.sockets) 
    //    {

    //        obj.transform.GetChild(0).Find(pair.Key).GetComponent<TextMeshProUGUI>().text = pair.Value;

    //    }

    //    obj2.transform.localRotation = Quaternion.Euler(new Vector3(0f, p.rotIndex * 90f, 0f));
    //    obj2.transform.localPosition = Vector3.zero;

    //    obj.transform.position = new Vector3(x * 4, 0f, z * 4);

    //}

    bool isValidNeighbor(string objectSocket, string potentialNeighbor)
    {
        if (objectSocket == "-1" || objectSocket.Contains("s") || objectSocket.Contains("v"))
        {
            if (objectSocket == potentialNeighbor)
                return true;
            else
                return false;
        }
        else if (objectSocket.Contains("f"))
        {

            if (potentialNeighbor == objectSocket.Remove(objectSocket.Length - 1))
                return true;
            else
                return false;
        }
        else if (!objectSocket.Contains("f"))
        {
            if (potentialNeighbor == objectSocket + "f")
                return true;
            else
                return false;
        }
        else
            return false;
            

    }

    public Dictionary<string, string> LabelSocket(Vector3[] vertices)
    {
        Dictionary<string, string> sockets = new Dictionary<string, string>
        {
            { "PosX", null },
            { "NegX", null },
            { "PosY", null },
            { "NegY", null },
            { "PosZ", null },
            { "NegZ", null },
        };

        List<Vector2> socketPosX = new List<Vector2>();
        List<Vector2> socketNegX = new List<Vector2>();
        List<Vector2> socketPosY = new List<Vector2>();
        List<Vector2> socketNegY = new List<Vector2>();
        List<Vector2> socketPosZ = new List<Vector2>();
        List<Vector2> socketNegZ = new List<Vector2>();


        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].x >= socketDistance)
            {
                socketPosX.Add(new Vector2(vertices[i].z, vertices[i].y));
            }
            else if (vertices[i].x <= -socketDistance)
            {
                socketNegX.Add(new Vector2(-vertices[i].z, vertices[i].y));
            }

            if (vertices[i].y >= socketDistance)
            {
                socketPosY.Add(new Vector2(vertices[i].x, vertices[i].z));
            }
            else if (vertices[i].y <= -socketDistance)
            {
                socketNegY.Add(new Vector2(vertices[i].x, vertices[i].z));
            }

            if (vertices[i].z >= socketDistance)
            {
                socketPosZ.Add(new Vector2(-vertices[i].x, vertices[i].y));
            }
            else if (vertices[i].z <= -socketDistance)
            {
                socketNegZ.Add(new Vector2(vertices[i].x, vertices[i].y));
            }
        }

        //Debug.Log("PosY");
        //foreach (Vector2 v in socketPosY)
        //{
        //    Debug.Log(v);
        //}

        //Debug.Log("NegY");
        //foreach (Vector2 v in socketNegY)
        //{
        //    Debug.Log(v);
        //}

        


        sockets["PosX"] = LabelHorizontalSockets(socketPosX);
        sockets["NegX"] = LabelHorizontalSockets(socketNegX);
        sockets["PosZ"] = LabelHorizontalSockets(socketPosZ);
        sockets["NegZ"] = LabelHorizontalSockets(socketNegZ);



        sockets["PosY"] = LabelVerticalSockets(socketPosY);
        sockets["NegY"] = LabelVerticalSockets(socketNegY);

        //foreach (var socket in sockets)
        //{
        //    Debug.Log(socket.Key + " = " + socket.Value);
        //}

        return sockets;
    }

    void CreatePrototypes(Dictionary<string, string> sockets, string meshName)
    {
       

        Prototype[] newPrototypes = new Prototype[4];

        newPrototypes[0] = new Prototype(); 

        newPrototypes[0].ID = prototypeCount;

        prototypeCount++;

        newPrototypes[0].meshName = meshName;

        newPrototypes[0].rotIndex = 0;

        newPrototypes[0].sockets = sockets;

        if (sockets["PosY"] != "-1")
            newPrototypes[0].sockets["PosY"] = sockets["PosY"] + "0";

        if (sockets["NegY"] != "-1")
            newPrototypes[0].sockets["NegY"] = sockets["NegY"] + "0";

        prototypes.Add(newPrototypes[0]);

        for (int i = 1; i < 4;  i++) 
        {
            newPrototypes[i] = new Prototype();

            newPrototypes[i].ID = prototypeCount;

            prototypeCount++;

            newPrototypes[i].meshName = meshName;

            newPrototypes[i].rotIndex = i;

            newPrototypes[i].sockets["PosX"] = newPrototypes[i - 1].sockets["PosZ"];

            newPrototypes[i].sockets["NegZ"] = newPrototypes[i - 1].sockets["PosX"];

            newPrototypes[i].sockets["NegX"] = newPrototypes[i - 1].sockets["NegZ"];

            newPrototypes[i].sockets["PosZ"] = newPrototypes[i - 1].sockets["NegX"];

            if (sockets["PosY"] != "-1")
                newPrototypes[i].sockets["PosY"] = sockets["PosY"] + i.ToString();
            else
                newPrototypes[i].sockets["PosY"] = sockets["PosY"];

            if (sockets["NegY"] != "-1")
                newPrototypes[i].sockets["NegY"] = sockets["NegY"] + i.ToString();

            else
                newPrototypes[i].sockets["NegY"] = sockets["NegY"];

            prototypes.Add(newPrototypes[i]);
        }

        
    }

    string LabelVerticalSockets(List<Vector2> socket)
    {
        string socketName = null;

        if (socket.Count == 0)
        {
           socketName = "-1";
        }
        else
        {
            string foundSocket = null;

            foreach (KeyValuePair<string, List<Vector2>> pair in socketListVertical)
            {
                if ((socket.Count != pair.Value.Count))
                    continue;

                else
                {
                    bool allFound = true;

                    foreach (Vector2 v in socket)
                    {
                        bool found = false;
                        foreach (Vector2 v2 in pair.Value)
                        {
                            if (v == v2)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            allFound = false;
                            break;
                        }
                    }

                    if (allFound)
                    {
                        foundSocket = pair.Key;
                        break;
                    }

                }

            }

            if (foundSocket != null)
                socketName = foundSocket;

            else
            {
                string newSocketKey = "v" + verticalSocketNumberCount + "_";
                socketListVertical.Add(newSocketKey, socket);
                socketName = newSocketKey;
                verticalSocketNumberCount++;
            }
        }

        return socketName;
    }

    string LabelHorizontalSockets(List<Vector2> socket )
    {
        string socketName = null;
        bool isSymetrical = IsSymetrical(socket);

        if (socket.Count == 0)
        {
           socketName = "-1";
        }
        else
        {
            string foundSocket = null;

            foreach (KeyValuePair<string, List<Vector2>> pair in socketList)
            {
                if (socket.Count != pair.Value.Count || pair.Key.Contains("v"))
                    continue;

                else
                {
                    bool allFound = true;

                    foreach (Vector2 v in socket)
                    {
                        bool found = false;
                        foreach (Vector2 v2 in pair.Value)
                        {
                            if (v == v2)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            allFound = false;
                            break;
                        }
                    }

                    if (allFound)
                    {
                        foundSocket = pair.Key;
                        break;
                    }

                }

            }

            if (foundSocket != null)
                socketName = foundSocket;

            else
            {
                string newSocketKey = socketNumberCount.ToString();

                if (!isSymetrical)
                {
                    string newStockFlippedKey = socketNumberCount.ToString() + "f";

                    List<Vector2> socketPosXFlipped = new List<Vector2>();

                    foreach (Vector2 v in socket)
                    {
                        Vector2 vFlipped = new Vector2(-v.x, v.y);
                        socketPosXFlipped.Add(vFlipped);
                    }
                    socketList.Add(newStockFlippedKey, socketPosXFlipped);
                }
                else
                    newSocketKey = newSocketKey + "s";

                socketName = newSocketKey;
                socketList.Add(newSocketKey, socket);
                socketNumberCount++;
            }
        }


        return socketName;
    }

    



    bool IsSymetrical(List<Vector2> socket)
    {
        int symetricCnt = 0;

        foreach (Vector2 v in socket)
        {
            Vector2 symetricVertex = new Vector2(-v.x, v.y);

            foreach (Vector2 v2 in socket)
            {
                if(symetricVertex == v2)
                {
                    symetricCnt++;
                    break;
                }
            }

        }

        if( symetricCnt == socket.Count ) 
            return true;

        else 
            return false;

    }
}

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
}