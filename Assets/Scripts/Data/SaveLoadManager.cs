using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager saveLoadManagerInstance { get; private set; }
    private List<ISaveable> saveables = new();
    private Dictionary<string, object> saveData = new();

    private string saveFilename = "shawarma.json";
    private string savePath;
    private string SavePath => savePath;
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
            return;
        }
        InitializeSavePathAsync().Forget();
    }
    private async UniTask InitializeSavePathAsync()
    {
        int retries = 0;
        while (string.IsNullOrEmpty(Application.persistentDataPath))
        {
            retries++;
            await UniTask.Delay(100);
            if (retries > 50)
            {
                Debug.LogError("persistentDataPath did not initialize within expected time.");
                return;
            }
        }
        savePath = Path.Combine(Application.persistentDataPath, saveFilename);
        Debug.Log($"SavePath set to: {savePath}");
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
        //  Debug.Log("Check Here");
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
        Debug.Log("Data Save Here");
    }

    internal void ResetAllISaveables()
    {
        foreach (ISaveable saveable in saveables)
        {
            //Debug.Log("Count");
            if (saveable.SaveKey != "player_progress")
                saveable.SetInitialData();
            else if (saveable.SaveKey == "player_progress")
            {
                PlayerProgress.Instance.ResetDataOnPrestigue();
            }


        }
        SaveGame();
    }


    internal async UniTask LoadGame()
    {
        //Create File if not exists
        await UniTask.WaitUntil(() => !string.IsNullOrWhiteSpace(SavePath));
        if (!File.Exists(SavePath))
        {
            string _json = JsonConvert.SerializeObject(saveData, jsonSettings);
            File.WriteAllText(SavePath, _json);
            // return;
        }
        print("Path " + SavePath);
        WaitForFileReady(SavePath);
        string json = File.ReadAllText(SavePath);
        saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, jsonSettings);
        foreach (ISaveable saveable in saveables)
        {
            //  print("key" + saveable.SaveKey);
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
        KitchenManager.Instance.DelayOnStart().Forget();
        WarehouseManager.Instance.DelayOnStart().Forget();
        CateringManager.Instance.DelayOnStart().Forget();
        DeliveryManager.Instance.DelayOnStart().Forget();
    }
    private void WaitForFileReady(string path)
    {
        const int maxRetries = 10; // prevent infinite loop
        int retries = 0;

        while (true)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (stream.Length > 0) // ensure file has content
                        break;
                }
            }
            catch (IOException)
            {
                // File is still locked or incomplete
            }

            retries++;
            if (retries >= maxRetries)
            {
                Debug.LogWarning($"WaitForFileReady: Timeout waiting for {path} to be ready.");
                break;
            }

            System.Threading.Thread.Sleep(50); // small delay before retry
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