using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Change to DontDestroyOnLoad approach later on to allow for versatility
public static class PersistentData
{
    private static Dictionary<string, PersistentDataBase> dataContainers = new();

    public static void AddDataContainer(PersistentDataBase data)
    {
        dataContainers.Add(data.GetType().ToString(), data);
    }

    public static PersistentDataBase RetrieveDataContainer(string name)
    {
        return dataContainers.ContainsKey(name) ? dataContainers[name] : null;
    }

    public static int Count()
    {
        return dataContainers.Count;
    }
}
