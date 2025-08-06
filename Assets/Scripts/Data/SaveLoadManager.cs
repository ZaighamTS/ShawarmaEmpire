using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager saveLoadManagerInstance { get; private set; }
    private List<ISaveable> saveables = new();
    private Dictionary<string, object> saveData = new();

    private string saveFilename = "shawarma.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, saveFilename);
    private readonly JsonSerializerSettings jsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented,
    };

    private void Awake()
    {
        if (saveLoadManagerInstance == null)
        {
            saveLoadManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Register(ISaveable saveable)
    {
        if (!saveables.Contains(saveable))
            saveables.Add(saveable);
    }
    public void Unregister(ISaveable saveable)
    {
        if (saveables.Contains(saveable))
            saveables.Remove(saveable);
    }
    internal void SaveGame()
    {
        Debug.Log("Check Here");
        foreach (ISaveable saveable in saveables)
        {
            if (saveable.IsDirty)
            {
                saveData[saveable.SaveKey] = saveable.CaptureState();
                saveable.ClearDirty();
            }
        }
        string json = JsonConvert.SerializeObject(saveData, jsonSettings);
        File.WriteAllText(SavePath, json);
    }
    internal void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            string _json = JsonConvert.SerializeObject(saveData, jsonSettings);
            File.WriteAllText(SavePath, _json);
           // return;
        }
        string json = File.ReadAllText(SavePath);
        saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, jsonSettings);
        foreach (ISaveable saveable in saveables)
        {
            print("key" + saveable.SaveKey);
            if (saveData.TryGetValue(saveable.SaveKey, out var state))
            {
                saveable.RestoreState(state);
            }
            else
            {
                Debug.Log("Else " + saveable.SaveKey);
                saveable.SetInitialData();
            }
        }
    }
}
public interface ISaveable
{
    public string SaveKey { get; }
    public bool IsDirty { get; }
    public object CaptureState();
    public void RestoreState(object state);
    public void SetInitialData();
    public void ClearDirty();
}