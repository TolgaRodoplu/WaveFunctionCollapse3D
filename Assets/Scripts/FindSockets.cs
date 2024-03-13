using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindSockets : MonoBehaviour
{
    int verticalSocketNumberCount = 0;
    int socketNumberCount = 0;
    float socketDistance = 0.99f;
    public List<MeshFilter> modules = new List<MeshFilter>();
    Dictionary<string, List<Vector2>> socketList;
    Dictionary<string, List<Vector2>> socketListVertical;
    List<Prototype> prototypes;
    int prototypeCount = 0;

    private void Start()
    {
        socketList = new Dictionary<string, List<Vector2>>();
        socketListVertical = new Dictionary<string, List<Vector2>>();
        prototypes = new List<Prototype>(); 

       

        foreach (var module in modules)
        {
            //Debug.Log(module.name);
            var sockets = LabelSocket(module.mesh.vertices);

            //foreach (var socket in sockets)
            //{
            //    Debug.Log(socket.Key + " = " + socket.Value);
            //}

            CreatePrototypes(sockets, module.mesh.name);
        }

        foreach (var p in prototypes)
        {
            Debug.Log(p.ID);
            Debug.Log(p.meshName);
            Debug.Log(p.rotIndex);
            foreach(var s in p.sockets)
                Debug.Log(s.Value);
        }

        //Fill valid neighbors
        foreach (Prototype p in prototypes)
        {
            foreach (Prototype p1 in prototypes)
            {
                if (isValidNeighbor(p.sockets["PosX"], p1.sockets["NegX"]))
                    p.validNeighbors["PosX"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["NegX"], p1.sockets["PosX"]))
                    p.validNeighbors["NegX"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["PosY"], p1.sockets["NegY"]))
                    p.validNeighbors["PosY"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["NegY"], p1.sockets["PosY"]))
                    p.validNeighbors["NegY"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["PosZ"], p1.sockets["NegZ"]))
                    p.validNeighbors["PosZ"].Add(p1.ID);

                if (isValidNeighbor(p.sockets["NegZ"], p1.sockets["PosZ"]))
                    p.validNeighbors["NegZ"].Add(p1.ID);
            }
        }

        //savePrototypesToJson
    }

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
