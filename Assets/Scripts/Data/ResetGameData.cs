using UnityEngine;

/// <summary>
/// Helper component to reset all game data.
/// Add this to any GameObject and use the context menu to reset.
/// Useful for testing and debugging.
/// </summary>
public class ResetGameData : MonoBehaviour
{
    [ContextMenu("Complete Reset (All Data)")]
    public void CompleteReset()
    {
        if (SaveLoadManager.saveLoadManagerInstance != null)
        {
            SaveLoadManager.saveLoadManagerInstance.CompleteReset();
            Debug.Log("✅ Complete reset done! All data cleared (PlayerPrefs + JSON file)");
        }
        else
        {
            Debug.LogError("SaveLoadManager not found! Make sure the game has started.");
        }
    }

    [ContextMenu("Delete Save File Only")]
    public void DeleteSaveFileOnly()
    {
        if (SaveLoadManager.saveLoadManagerInstance != null)
        {
            SaveLoadManager.saveLoadManagerInstance.DeleteSaveFile();
            Debug.Log("✅ Save file deleted!");
        }
        else
        {
            Debug.LogError("SaveLoadManager not found! Make sure the game has started.");
        }
    }

    [ContextMenu("Clear PlayerPrefs Only")]
    public void ClearPlayerPrefsOnly()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("✅ PlayerPrefs cleared!");
    }

    [ContextMenu("Show Save File Path")]
    public void ShowSaveFilePath()
    {
        if (SaveLoadManager.saveLoadManagerInstance != null)
        {
            string path = SaveLoadManager.saveLoadManagerInstance.GetSaveFilePath();
            Debug.Log($"Save file path: {path}");
            Debug.Log($"File exists: {System.IO.File.Exists(path)}");
        }
        else
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "shawarma.json");
            Debug.Log($"Save file path (estimated): {path}");
            Debug.Log($"File exists: {System.IO.File.Exists(path)}");
        }
    }
}

