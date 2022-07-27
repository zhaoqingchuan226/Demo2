using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSystem
{
    public static void SaveByJson(string saveFileName, object data)//将object存到json中
    {
        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(path, json);
    }

    public static T LoadFromJson<T>(string saveFileName)
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<T>(json);
        return data;
    }


    public static void DeleteSaveFile(string saveFileName)
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.Delete(path);
    }
}
