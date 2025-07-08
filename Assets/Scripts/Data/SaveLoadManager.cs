using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private void Awake()
    {

    }
    internal void SaveGame()
    {

    }
    internal void LoadGame()
    {

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