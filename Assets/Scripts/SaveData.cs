using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveData 
{
    static string filePath = Application.persistentDataPath + "/ModuleData.json";
    public static void SaveToJson(ModuleList modules)
    {
        string moduleData = JsonConvert.SerializeObject(modules);
        System.IO.File.WriteAllText(filePath, moduleData);
        Debug.Log("Prototypes for all the meshes have been created and saved to the directory: " + filePath);
    }

    public static ModuleList LoadFromJson()
    {
        string moduleData = System.IO.File.ReadAllText(filePath);
        ModuleList modules = JsonConvert.DeserializeObject<ModuleList>(moduleData);
        return modules;
    }
}
