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
        // Phase 0: Record this session's login time (for gift calendar and stats)
        if (PlayerProgress.Instance != null)
            PlayerProgress.Instance.SetLastLoginUtc(System.DateTime.UtcNow.ToString("o"));

        KitchenManager.Instance.DelayOnStart().Forget();
        WarehouseManager.Instance.DelayOnStart().Forget();
        CateringManager.Instance.DelayOnStart().Forget();
        DeliveryManager.Instance.DelayOnStart().Forget();
        
        // Check for offline earnings after game loads
        if (GameManager.gameManagerInstance != null)
        {
            // Delay offline earnings check so managers (e.g. WarehouseManager.DelayOnStart) can initialize
            // and so we run before GameManager's periodic CurrentDateTime refresh (grace period ~20s)
            await UniTask.Delay(1500);
            GameManager.gameManagerInstance.CheckOfflineEarning();
        }
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

    /// <summary>
    /// Deletes the JSON save file from disk.
    /// Call this to completely clear saved game data.
    /// </summary>
    public void DeleteSaveFile()
    {
        if (string.IsNullOrEmpty(SavePath))
        {
            Debug.LogWarning("SavePath not initialized yet. Save file path: " + 
                Path.Combine(Application.persistentDataPath, saveFilename));
            return;
        }

        if (File.Exists(SavePath))
        {
            try
            {
                File.Delete(SavePath);
                Debug.Log($"Save file deleted: {SavePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to delete save file: {e.Message}");
            }
        }
        else
        {
            Debug.Log($"Save file does not exist: {SavePath}");
        }
    }

    /// <summary>
    /// Completely resets all game data:
    /// 1. Deletes JSON save file
    /// 2. Clears PlayerPrefs
    /// 3. Resets all ISaveables to initial state
    /// 4. Clears in-memory save data
    /// </summary>
    public void CompleteReset()
    {
        Debug.Log("=== COMPLETE GAME RESET ===");
        
        // 1. Delete JSON save file
        DeleteSaveFile();
        
        // 2. Clear PlayerPrefs (including CurrentDateTime for offline earnings)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteKey("CurrentDateTime"); // Explicitly clear offline time tracking
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared (including CurrentDateTime)");
        
        // 3. Clear in-memory save data
        saveData.Clear();
        Debug.Log("In-memory save data cleared");
        
        // 4. Reset all ISaveables to initial state
        foreach (ISaveable saveable in saveables)
        {
            saveable.SetInitialData();
            Debug.Log($"Reset ISaveable: {saveable.SaveKey}");
        }
        
        // 5. Create fresh save file with initial data
        SaveGame();
        Debug.Log("Fresh save file created with initial data");
        Debug.Log("=== RESET COMPLETE ===");
    }

    /// <summary>
    /// Gets the full path to the save file.
    /// Useful for debugging or manual file deletion.
    /// </summary>
    public string GetSaveFilePath()
    {
        if (string.IsNullOrEmpty(SavePath))
        {
            return Path.Combine(Application.persistentDataPath, saveFilename);
        }
        return SavePath;
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